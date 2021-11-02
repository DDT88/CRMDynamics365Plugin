using System;
using System.Linq;
using DemoEolo;
using Microsoft.Xrm.Sdk;

namespace ListinoAPI
{
    public class Res_creazioneListinoApi : IPlugin
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

            if (context.MessageName.Equals("res_creazioneListinoApi") && context.Stage.Equals(30))
            {

                try
                {
                    Entity wholesaler = (Entity)context.InputParameters["Wholesaler"];
                    EntityCollection listaServizi = (EntityCollection)context.InputParameters["ListinoServiziRequest"];

                    EntityReference account = utility.getAccount(service, wholesaler.GetAttributeValue<string>("name"));
                    
                    //check/crezione nuovo wholesaler
                    if(account == null)
                    {
                        wholesaler.Attributes.Add("res_wholesaler", true);
                        Guid wholesalerCreatedId = service.Create(wholesaler);

                        account = new EntityReference("account", wholesalerCreatedId);
                    }

                    //check/creazione listino padre wholesaler
                    EntityCollection listaListiniPadre = utility.getListinoPrezziFromWholesaler(service, account,true);
                    EntityReference listinoPadre = null;

                    if (listaListiniPadre == null || (listaListiniPadre.Entities.Count == 0))
                    {
                        Entity createdListinoPadre = new Entity("pricelevel");

                        createdListinoPadre.Attributes.Add("name", "Listino - " + wholesaler.GetAttributeValue<string>("name"));
                        createdListinoPadre.Attributes.Add("res_wholesalerid", account);

                        Guid idListinoPadre = service.Create(createdListinoPadre);

                        listinoPadre = new EntityReference("pricelevel", idListinoPadre);
                    }
                    else
                    {
                        listinoPadre = listaListiniPadre.Entities[0].ToEntityReference();
                    }


                    //creazione listino servizi

                    if (listaServizi != null && listaServizi.Entities.Count > 0)
                    {
                        foreach (Entity servizio in listaServizi.Entities)
                        {
                            string nomeServizio = servizio.GetAttributeValue<string>("res_name");

                            EntityReference listinoFiglio = utility.getListinoPrezzi(service, account, nomeServizio);

                            //creo listino figlio se non è presente
                            if(listinoFiglio == null)
                            {
                                Entity newListinoFiglio = new Entity("pricelevel");

                                newListinoFiglio.Attributes.Add("name", "Listino " + nomeServizio + " - " + wholesaler.GetAttributeValue<string>("name"));
                                newListinoFiglio.Attributes.Add("res_listinopadreid", listinoPadre);
                                newListinoFiglio.Attributes.Add("res_wholesalerid", account);

                                Guid idListinoFiglio = service.Create(newListinoFiglio);

                                listinoFiglio = new EntityReference("pricelevel", idListinoFiglio);

                                //creazione voci di listino

                                //recuperare i product dal prodotto padre
                                EntityReference prodottoPadre = utility.getProdottoPadre(service, nomeServizio);

                                if(prodottoPadre != null)
                                {
                                    EntityCollection listaProdottiFigli = utility.getProdottiFiglio(service, prodottoPadre);

                                    if(listaProdottiFigli != null && listaProdottiFigli.Entities.Count > 0)
                                    {
                                        foreach (Entity prodottoFiglio in listaProdottiFigli.Entities)
                                        {
                                            //creazione voce listino
                                            Entity voceListino = new Entity("productpricelevel");

                                            voceListino.Attributes.Add("pricelevelid", listinoFiglio);
                                            voceListino.Attributes.Add("productid", prodottoFiglio.ToEntityReference());
                                            voceListino.Attributes.Add("pricingmethodcode", new OptionSetValue(1));
                                            voceListino.Attributes.Add("uomid", utility.getUnita(service));
                                            

                                            if (prodottoFiglio.GetAttributeValue<string>("name").Contains("Canone"))
                                            {
                                                voceListino.Attributes.Add("amount", servizio.GetAttributeValue<Money>("res_canone"));
                                                voceListino.Attributes.Add("res_periodicitfatturazione", new OptionSetValue(283240001));
                                            }
                                            else
                                            {
                                                voceListino.Attributes.Add("amount", servizio.GetAttributeValue<Money>("res_contributo"));
                                                voceListino.Attributes.Add("res_periodicitfatturazione", new OptionSetValue(283240000));
                                            }

                                            service.Create(voceListino);

                                        }
                                    }
                                }
                                else
                                {
                                    throw new InvalidPluginExecutionException("Servizio non configurato");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Lista servizi vuota");
                    }

                    context.OutputParameters["Esito"] = "Listino Creato";

                }
                catch (Exception ex)
                {
                    tracingService.Trace("res_creazioneListinoApi: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
            else
            {
                tracingService.Trace("res_creazioneListinoApi plug-in is not associated with the expected message or is not registered for the main operation.");
            }
        }
    }
}