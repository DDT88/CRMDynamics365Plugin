using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEolo
{
    public class Utility
    {
        public EntityCollection getVociListino(IOrganizationService service,string listinoId)
        {
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='productpricelevel'>" +
                                                                "<all-attributes/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='pricelevelid' operator='eq' value='" + listinoId + "' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaProduct = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));


            return listaProduct;
        }

        public EntityCollection getProductsOrder(IOrganizationService service, string orderId)
        {
            String fetchXMlOrdineProdotti = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                             "<entity name='salesorderdetail'>" +
                                                                 "<all-attributes/>" +
                                                                  "<filter>" +
                                                                      "<condition attribute='salesorderid' operator='eq' value='" + orderId + "' />" +
                                                                  "</filter>" +
                                                                 "</entity>" +
                                                             "</fetch>";

            EntityCollection listaOrdiniProdotti = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdineProdotti.ToString()));


            return listaOrdiniProdotti;
        }

        public bool checkProductsInvoice(IOrganizationService service, string orderId,string productId)
        {
            bool ret = false;
            String fetchXMlProductInvoice = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                             "<entity name='invoicedetail'>" +
                                                                 "<attribute name='invoicedetailid'/>" +
                                                                  "<filter>" +
                                                                      "<condition attribute='res_ordineid' operator='eq' value='" + orderId + "' />" +
                                                                      "<condition attribute='productid' operator='eq' value='" + productId + "' />" +
                                                                  "</filter>" +
                                                                 "</entity>" +
                                                             "</fetch>";

            EntityCollection listaProductInvoice = service.RetrieveMultiple(new FetchExpression(fetchXMlProductInvoice.ToString()));

            if (listaProductInvoice != null && listaProductInvoice.Entities.Count > 0)
                ret = true;


            return ret;
        }

        public EntityReference getAccount(IOrganizationService service, string nomeAccount)
        {
            EntityReference ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='account'>" +
                                                                "<attribute name='accountid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='name' operator='eq' value='" + nomeAccount + "' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaAccount = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaAccount != null && listaAccount.Entities.Count > 0)
                ret = listaAccount.Entities[0].ToEntityReference();


            return ret;
        }

        public EntityReference getListinoPrezzi(IOrganizationService service, string listinoPrezzi)
        {
            EntityReference ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='pricelevel'>" +
                                                               "<attribute name='pricelevelid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='name' operator='eq' value='" + listinoPrezzi + "' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaListini = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaListini != null && listaListini.Entities.Count > 0)
                ret = listaListini.Entities[0].ToEntityReference();

            return ret;
        }


        public EntityCollection getListinoPrezziFromWholesaler(IOrganizationService service, EntityReference wholesaler,bool isPadre)
        {
            EntityCollection ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='pricelevel'>" +
                                                               "<attribute name='pricelevelid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='res_wholesalerid' operator='eq' value='" + wholesaler.Id.ToString() + "' />" +
                                                                     "<condition attribute='res_listinopadreid' operator='"+ (isPadre ? "null" : "not-null")  +"' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaListini = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaListini != null && listaListini.Entities.Count > 0)
                ret = listaListini;

            return ret;
        }

        public EntityReference getListinoPrezzi(IOrganizationService service, EntityReference wholesaler, string nomeServizio)
        {
            EntityReference ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='pricelevel'>" +
                                                               "<attribute name='pricelevelid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='res_wholesalerid' operator='eq' value='" + wholesaler.Id.ToString() + "' />" +
                                                                     "<condition attribute='res_listinopadreid' operator='not-null' />" +
                                                                     "<condition attribute='name' operator='like' value='%" + nomeServizio + "%' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaListini = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaListini != null && listaListini.Entities.Count > 0)
                ret = listaListini.Entities[0].ToEntityReference();

            return ret;
        }

        public EntityReference getProdottoPadre(IOrganizationService service, string nomeServizio)
        {
            EntityReference ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='product'>" +
                                                               "<attribute name='productid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='name' operator='eq' value='" + nomeServizio + "' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaProdotti = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaProdotti != null && listaProdotti.Entities.Count > 0)
                ret = listaProdotti.Entities[0].ToEntityReference();

            return ret;
        }

        public EntityCollection getProdottiFiglio(IOrganizationService service, EntityReference prodottoPadre)
        {
            EntityCollection ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='product'>" +
                                                               "<attribute name='productid'/>" +
                                                               "<attribute name='name'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='parentproductid' operator='eq' value='" + prodottoPadre.Id.ToString() + "' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaListini = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaListini != null && listaListini.Entities.Count > 0)
                ret = listaListini;

            return ret;
        }

        public EntityCollection getOrdiniDaFatturare(IOrganizationService service,string wholesalerId)
        {
            EntityCollection ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='salesorder'>" +
                                                               "<all-attributes/>" +
                                                               "<filter type='and'>" +
                                                               "<condition attribute='customerid' operator='eq' value='" + wholesalerId + "' />" +
                                                                 "<filter type='or'>" +
                                                                     "<condition attribute='statuscode' operator='eq' value='100001' />" + //Attivo
                                                                     "<condition attribute='statuscode' operator='eq' value='100003' />" + //Fatturato
                                                                     "<condition attribute='statuscode' operator='eq' value='283240002' />" + //Inviato fatturazione
                                                                 "</filter>" +
                                                               "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaOrdini = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaOrdini != null && listaOrdini.Entities.Count > 0)
                ret = listaOrdini;

            return ret;
        }

        public EntityReference getUnita(IOrganizationService service)
        {
            EntityReference ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='uom'>" +
                                                               "<attribute name='uomid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='name' operator='eq' value='Unità primaria' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaListini = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaListini != null && listaListini.Entities.Count > 0)
                ret = listaListini.Entities[0].ToEntityReference();

            return ret;
        }

        public EntityReference retrieveSedeClienteFinale(IOrganizationService service, string nomeSedeClienteFinale)
        {
            EntityReference ret = null;
            String fetchXMlOrdine = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='res_sede'>" +
                                                                "<attribute name='res_sedeid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='res_name' operator='eq' value='" + nomeSedeClienteFinale + "' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaSedi = service.RetrieveMultiple(new FetchExpression(fetchXMlOrdine.ToString()));

            if (listaSedi != null && listaSedi.Entities.Count > 0)
                ret = listaSedi.Entities[0].ToEntityReference();

            return ret;
        }

        public EntityReference getFatturaWholesaler(IOrganizationService service,EntityReference wholesaler)
        {
            EntityReference ret = null;
            String fetchXMlFatture = "<fetch mapping='logical' count='0' version='1.0'>" +
                                                            "<entity name='invoice'>" +
                                                                "<attribute name='invoiceid'/>" +
                                                                 "<filter>" +
                                                                     "<condition attribute='customerid' operator='eq' value='" + wholesaler.Id.ToString() + "' />" +
                                                                     "<condition attribute='createdon' operator='today' />" +
                                                                 "</filter>" +
                                                                "</entity>" +
                                                            "</fetch>";

            EntityCollection listaFatture = service.RetrieveMultiple(new FetchExpression(fetchXMlFatture.ToString()));

            if (listaFatture != null && listaFatture.Entities.Count > 0)
                ret = listaFatture.Entities[0].ToEntityReference();

            return ret;
        }
    }
}
