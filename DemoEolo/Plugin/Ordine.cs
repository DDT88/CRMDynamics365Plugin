using DemoEolo;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Text;

namespace Ordine
{
    public class PostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                if (context.MessageName != "Create" || context.PrimaryEntityName != "salesorder")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Plug-in non registrato correttamente. Context.Mode: {0}; Context.Stage: {1}; Context.MessageName: {2}; Context.PrimaryEntityName: {3}", context.Mode.ToString(), context.Stage.ToString(), context.MessageName, context.PrimaryEntityName);
                    throw new ApplicationException(sb.ToString());
                }

                if (!context.InputParameters.Contains("Target"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Plugin Non contiene Target  Context.InputParameters.Contains(\"Target\"): {0};", context.InputParameters.Contains("Target").ToString());
                    throw new ApplicationException(sb.ToString());
                }

                if (!(context.InputParameters["Target"] is Entity))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Target non è un entità Context.InputParameters[\"Target\"].GetType(): {0}", context.InputParameters["Target"].GetType().FullName);
                    throw new ApplicationException(sb.ToString());
                }

                Entity currEntity;
                service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                currEntity = (Entity)context.InputParameters["Target"];
                Utility utility = new Utility();

                //recupero i prodotto dal listino prodotti tramite entityreference

                EntityReference listinoId = currEntity.GetAttributeValue<EntityReference>("pricelevelid");

                if(listinoId != null)
                {
                    EntityCollection listaVociListino = utility.getVociListino(service, listinoId.Id.ToString());

                    if(listaVociListino != null && listaVociListino.Entities.Count > 0)
                    {
                        //creo record prodotti ordine prendendo i dati da prodotto listino
                        foreach (Entity voceListino in listaVociListino.Entities)
                        {
                            try
                            {
                                Entity prodottoOrdine = new Entity("salesorderdetail");

                                prodottoOrdine.Attributes.Add("salesorderid", currEntity.ToEntityReference());
                                prodottoOrdine.Attributes.Add("productid", voceListino.GetAttributeValue<EntityReference>("productid"));
                                prodottoOrdine.Attributes.Add("uomid", voceListino.GetAttributeValue<EntityReference>("uomid"));
                                prodottoOrdine.Attributes.Add("priceperunit", voceListino.GetAttributeValue<Money>("amount"));
                                prodottoOrdine.Attributes.Add("res_periodicitafatturazione", voceListino.GetAttributeValue<OptionSetValue>("res_periodicitfatturazione"));

                                if (voceListino.Attributes.Contains("productidname") && voceListino.GetAttributeValue<string>("productidname").Contains("Canone"))
                                    prodottoOrdine.Attributes.Add("quantity", 12M);
                                else
                                    prodottoOrdine.Attributes.Add("quantity", 1M);

                                service.Create(prodottoOrdine);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidPluginExecutionException("Errore nella creazione dei prodotti ordine. \n" + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Il listino selezionato non presenta prodotti configurabili.");
                    }
                }

            }
            catch (ApplicationException ex)
            {
                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                //   throw ex;
                throw new InvalidPluginExecutionException(ex.ToString(), ex);
            }
        }
    }

    public class PostUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                if (context.MessageName != "Update" || context.PrimaryEntityName != "salesorder")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Plug-in non registrato correttamente. Context.Mode: {0}; Context.Stage: {1}; Context.MessageName: {2}; Context.PrimaryEntityName: {3}", context.Mode.ToString(), context.Stage.ToString(), context.MessageName, context.PrimaryEntityName);
                    throw new ApplicationException(sb.ToString());
                }

                if (!context.InputParameters.Contains("Target"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Plugin Non contiene Target  Context.InputParameters.Contains(\"Target\"): {0};", context.InputParameters.Contains("Target").ToString());
                    throw new ApplicationException(sb.ToString());
                }

                if (!(context.InputParameters["Target"] is Entity))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Target non è un entità Context.InputParameters[\"Target\"].GetType(): {0}", context.InputParameters["Target"].GetType().FullName);
                    throw new ApplicationException(sb.ToString());
                }

                Entity currEntity;
                service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                currEntity = (Entity)context.InputParameters["Target"];
                Utility utility = new Utility();
                //recupero i prodotto dal listino prodotti tramite entityreference

                EntityReference listinoId = currEntity.GetAttributeValue<EntityReference>("pricelevelid");

                if (listinoId != null)
                {
                    EntityCollection listaVociListino = utility.getVociListino(service, listinoId.Id.ToString());

                    if (listaVociListino != null && listaVociListino.Entities.Count > 0)
                    {
                        //essendo in update prima di creare nuovi prodotti ordine cancello quelli precedenti

                        EntityCollection listaOrdiniProdotti = utility.getProductsOrder(service, currEntity.Id.ToString());

                        if (listaOrdiniProdotti != null && listaOrdiniProdotti.Entities.Count > 0)
                        {
                            foreach (Entity ordineProdotto in listaOrdiniProdotti.Entities)
                            {
                                service.Delete("salesorderdetail", ordineProdotto.Id);
                            }
                        }

                        //creo record prodotti ordine prendendo i dati da prodotto listino
                        foreach (Entity voceListino in listaVociListino.Entities)
                        {

                            try
                            {
                                Entity prodottoOrdine = new Entity("salesorderdetail");

                                prodottoOrdine.Attributes.Add("salesorderid", currEntity.ToEntityReference());
                                prodottoOrdine.Attributes.Add("productid", voceListino.GetAttributeValue<EntityReference>("productid"));
                                prodottoOrdine.Attributes.Add("uomid", voceListino.GetAttributeValue<EntityReference>("uomid"));
                                prodottoOrdine.Attributes.Add("priceperunit", voceListino.GetAttributeValue<Money>("amount"));
                                prodottoOrdine.Attributes.Add("res_periodicitafatturazione", voceListino.GetAttributeValue<OptionSetValue>("res_periodicitfatturazione"));

                                if (voceListino.Attributes.Contains("name") && voceListino.GetAttributeValue<string>("name").Contains("Canone"))
                                    prodottoOrdine.Attributes.Add("quantity", 12M);
                                else
                                    prodottoOrdine.Attributes.Add("quantity", 1M);

                                service.Create(prodottoOrdine);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidPluginExecutionException("Errore nella creazione dei prodotti ordine. \n" + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Il listino selezionato non presenta prodotti configurabili.");
                    }
                }
            }
            catch (ApplicationException ex)
            {
                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                //   throw ex;
                throw new InvalidPluginExecutionException(ex.ToString(), ex);
            }
        }
    }
}
