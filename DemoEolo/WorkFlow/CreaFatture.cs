using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEolo.WorkFlow
{
    public class CreaFatture : CodeActivity
    {
        [Input("Wholesaler")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> Wholesaler { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            EntityReference wholesaler = executionContext.GetValue(this.Wholesaler);

            Utility utility = new Utility();

            //recuperare tutti gli ordini con stato attivato e fatturato e inviato in fatturazione Attivo=100001,Fatturato=100003,InviatoFatt=283240002

            EntityCollection listaOrdiniDaFatturare = utility.getOrdiniDaFatturare(service, wholesaler.Id.ToString());

            if(listaOrdiniDaFatturare != null && listaOrdiniDaFatturare.Entities.Count > 0)
            {
                foreach (Entity ordine in listaOrdiniDaFatturare.Entities)
                {
                    //controllare se esiste il record fattura per il wholesaler, se c'è vado in aggiunta dei prodotti ordine
                    EntityReference fattura = utility.getFatturaWholesaler(service, ordine.GetAttributeValue<EntityReference>("customerid"));

                    if(fattura == null)
                    {
                        //se non esiste creo il record fattura per il wholesaler

                        EntityReference listinoFiglio = ordine.GetAttributeValue<EntityReference>("pricelevelid");

                        if(listinoFiglio != null)
                        {
                            Entity newFattura = new Entity("invoice");

                            newFattura.Attributes.Add("name", "Fattura " + ordine.GetAttributeValue<EntityReference>("customerid").Name + " - " + DateTime.Now.ToString("ddMMyyyy"));
                            newFattura.Attributes.Add("pricelevelid", listinoFiglio);


                            if(ordine.GetAttributeValue<EntityReference>("customerid") != null)
                                newFattura.Attributes.Add("customerid", ordine.GetAttributeValue<EntityReference>("customerid"));
                            else if(ordine.GetAttributeValue<EntityReference>("res_wholesalerid") != null)
                                newFattura.Attributes.Add("customerid", ordine.GetAttributeValue<EntityReference>("res_wholesalerid"));
                            //newFattura.Attributes.Add("discountpercentage", ordine.GetAttributeValue<decimal>("discountpercentage"));

                            Guid idFattura = service.Create(newFattura);

                            fattura = new EntityReference("invoice", idFattura);
                        }
                    }

                    //recupero i prodotti ordine
                    EntityCollection prodottiOrdine = utility.getProductsOrder(service, ordine.Id.ToString());

                    if(prodottiOrdine != null && prodottiOrdine.Entities.Count > 0)
                    {
                        foreach (Entity prodotto in prodottiOrdine.Entities)
                        {
                            if(!utility.checkProductsInvoice(service, ordine.Id.ToString(), prodotto.GetAttributeValue<EntityReference>("productid").Id.ToString())) // controllo che quell'ordine non sia gia inserito in fattura
                            {
                                Entity prodottoFattura = new Entity("invoicedetail");

                                prodottoFattura.Attributes.Add("invoicedetailname", prodotto.GetAttributeValue<string>("salesorderdetailname"));
                                prodottoFattura.Attributes.Add("productid", prodotto.GetAttributeValue<EntityReference>("productid"));
                                prodottoFattura.Attributes.Add("res_ordineid", ordine.ToEntityReference());
                                prodottoFattura.Attributes.Add("invoiceid", fattura);
                                prodottoFattura.Attributes.Add("priceperunit", prodotto.GetAttributeValue<Money>("priceperunit"));
                                //prodottoFattura.Attributes.Add("baseamount", prodotto.GetAttributeValue<Money>("baseamount"));
                                //prodottoFattura.Attributes.Add("extendedamount", prodotto.GetAttributeValue<Money>("extendedamount"));
                                prodottoFattura.Attributes.Add("uomid", prodotto.GetAttributeValue<EntityReference>("uomid"));
                                prodottoFattura.Attributes.Add("quantity", prodotto.GetAttributeValue<decimal>("quantity"));
                                //prodottoFattura.Attributes.Add("volumediscountamount", new Money(15));
                                prodottoFattura.Attributes.Add("manualdiscountamount", prodotto.GetAttributeValue<Money>("manualdiscountamount"));
                                prodottoFattura.Attributes.Add("tax", prodotto.GetAttributeValue<Money>("tax"));
                                prodottoFattura.Attributes.Add("ispriceoverridden", prodotto.GetAttributeValue<bool>("ispriceoverridden"));

                                //tracingService.Trace("creafatture: sconto prima del salvataggio: {0}", prodottoFattura.GetAttributeValue<Money>("volumediscountamount").Value.ToString());

                                service.Create(prodottoFattura);
                            }
                        }

                        //imposto lo stato dell'ordine in fatturato statuscode:100003, statecode=4
                        if (ordine.Attributes.Contains("statuscode") && ordine.GetAttributeValue<OptionSetValue>("statuscode").Value != 100003)
                        {
                            SetStateRequest setState = new SetStateRequest();
                            setState.EntityMoniker = ordine.ToEntityReference();
                            setState.State = new OptionSetValue(4);
                            setState.Status = new OptionSetValue(100003);
                            SetStateResponse setStateResponse = (SetStateResponse)service.Execute(setState);

                        }
                    }
                }
            }
            else
            {
                //nulla da fatturare
            }
        }
    }
}
