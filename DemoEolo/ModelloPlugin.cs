using Microsoft.Xrm.Sdk;
using System;
using System.Text;

namespace ModelloPlugin
{
    internal class PreCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                if (context.MessageName != "Create" || context.PrimaryEntityName != "biz_consistenza")
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
                /*
                NewOrUpdatedItem upd = new NewOrUpdatedItem(ChangeType.NewOrUpdated, currEntity);
                Microsoft.Xrm.Sdk.Messages.UpsertRequest upSert = new Microsoft.Xrm.Sdk.Messages.UpsertRequest();
                upSert.Target = currEntity;
                service.Execute(upSert);
                */
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

    internal class PostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                if (context.MessageName != "Create" || context.PrimaryEntityName != "biz_consistenza")
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

    internal class PreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                if (context.MessageName != "Update" || context.PrimaryEntityName != "biz_consistenza")
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

    internal class PostUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                if (context.MessageName != "Update" || context.PrimaryEntityName != "biz_consistenza")
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

    internal class PreDelete : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                if (context.MessageName != "Delete" || context.PrimaryEntityName != "biz_consistenza")
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

                if (!(context.InputParameters["Target"] is EntityReference))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Target non è un entità Context.InputParameters[\"Target\"].GetType(): {0}", context.InputParameters["Target"].GetType().FullName);
                    throw new ApplicationException(sb.ToString());
                }

                service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                EntityReference currRefEntity = (EntityReference)context.InputParameters["Target"];
            }
            catch (ApplicationException ex)
            {
                throw new InvalidPluginExecutionException("The plug-in terminated with an error: " + Environment.NewLine + ex.Message, ex);
            }
            catch (Exception ex)
            {
                //   throw ex;
                throw new InvalidPluginExecutionException("Generic Exception: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString(), ex);
            }
        }
    }

    internal class Service
    {
    }
}