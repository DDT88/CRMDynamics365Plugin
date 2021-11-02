using System;
using System.Linq;
using DemoEolo;
using Microsoft.Xrm.Sdk;

namespace OrdineAPI
{
    public class Res_creazioneOrdineApi : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);

            Utility utility = new Utility();

            if ((context.MessageName.Equals("res_creazioneOrdineApi") || context.MessageName.Equals("res_creazioneOrdineApiFunction")) && context.Stage.Equals(30))
            {

                try
                {
                    string nomeOrdine = (string)context.InputParameters["NomeOrdine"];
                    string wholesaler = (string)context.InputParameters["Wholesaler"];
                    string clienteFinale = (string)context.InputParameters["ClienteFinale"];
                    string servizio = (string)context.InputParameters["Servizio"];
                    Entity sedeClienteFinale = (Entity)context.InputParameters["SedeClienteFinale"];

                    //wholesaler, stringa recuperare entità account
                    //cliente finale, stringa recuperare entità account
                    //listino prezzi, stringa recuperare entità account

                    //dati sede


                    if (!string.IsNullOrEmpty(nomeOrdine))
                    {
                        EntityReference wholesalerEr = utility.getAccount(service, wholesaler);

                        if(wholesalerEr == null)
                            throw new InvalidPluginExecutionException("Nome Wholesaler non trovato nel CRM.");

                        EntityReference clienteFinaleEr = utility.getAccount(service, clienteFinale);

                        if (clienteFinaleEr == null)
                        {
                            Entity createClienteFinale = new Entity("account");

                            createClienteFinale.Attributes.Add("name", clienteFinale);

                            Guid idClienteFinale = service.Create(createClienteFinale);

                            clienteFinaleEr = new EntityReference("account", idClienteFinale);
                        }

                        Entity w = service.Retrieve("account", wholesalerEr.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                        tracingService.Trace("res_creazioneOrdineApi: wholesaler: {0}", w.GetAttributeValue<string>("name"));
                        tracingService.Trace("res_creazioneOrdineApi: servizio: {0}", servizio);

                        EntityReference listinoPrezziEr = utility.getListinoPrezzi(service, wholesalerEr, servizio);

                        if (listinoPrezziEr == null)
                            throw new InvalidPluginExecutionException("Listino Prezzi non trovato nel CRM.");


                        //controllo su sede
                        EntityReference sede = utility.retrieveSedeClienteFinale(service, sedeClienteFinale.GetAttributeValue<string>("res_name"));
                        Guid? sedeCreataId = null;
                        if (sede == null) // se non esiste la creo
                        {
                            sedeClienteFinale.Attributes.Add("res_accountid", clienteFinaleEr);
                            sedeCreataId = service.Create(sedeClienteFinale);
                        }
                        else
                        {
                            sedeCreataId = sede.Id;
                        }

                        Entity newOrdine = new Entity("salesorder");

                        Random rnd = new Random();
                        int code = rnd.Next(10000, 20000);

                        newOrdine.Attributes.Add("name", nomeOrdine + "-API-" + code.ToString());
                        newOrdine.Attributes.Add("customerid", wholesalerEr);
                        newOrdine.Attributes.Add("res_wholesaler", wholesalerEr);
                        newOrdine.Attributes.Add("res_clientefinaleid", clienteFinaleEr);
                        newOrdine.Attributes.Add("pricelevelid", listinoPrezziEr);
                        newOrdine.Attributes.Add("res_sedeid", new EntityReference("res_sede", sedeCreataId.Value));

                        service.Create(newOrdine);

                        context.OutputParameters["Esito"] = "Ordine Creato";
                    }

                }
                catch (Exception ex)
                {
                    tracingService.Trace("res_creazioneOrdineApi: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
            else
            {
                tracingService.Trace("res_creazioneOrdineApi plug-in is not associated with the expected message or is not registered for the main operation.");
            }
        }
    }
}