﻿namespace Merchello.Web.Trees
{
    using System.Net.Http.Formatting;

    using Merchello.Core.Configuration;
    using Merchello.Core.Configuration.Outline;

    using umbraco;
    using umbraco.BusinessLogic.Actions;
    using Umbraco.Web.Models.Trees;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Trees;

    /// <summary>
    /// The merchello tree controller.
    /// </summary>
    [Tree("merchello", "merchello", "Merchello")]
    [PluginController("Merchello")]
    public class MerchelloTreeController : TreeController
    {
        /// <summary>
        /// The get tree nodes.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNodeCollection"/>.
        /// </returns>
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var collection = new TreeNodeCollection();

            var backoffice = MerchelloConfiguration.Current.BackOffice;

            switch (id)
            {
                case "settings":
                    collection.Add(CreateTreeNode("shipping", "settings", queryStrings, "Shipping", "icon-truck", false, "merchello/merchello/Shipping/manage"));
                    collection.Add(CreateTreeNode("taxation", "settings", queryStrings, "Taxation", "icon-piggy-bank", false, "merchello/merchello/Taxation/manage"));
                    collection.Add(CreateTreeNode("payment", "settings", queryStrings, "Payment", "icon-bill-dollar", false, "merchello/merchello/Payment/manage"));
                    collection.Add(CreateTreeNode("notifications", "settings", queryStrings, "Notifications", "icon-chat", false, "merchello/merchello/Notifications/manage"));
                    collection.Add(CreateTreeNode("gateways", "settings", queryStrings, "Gateway Providers", "icon-trafic", false, "merchello/merchello/GatewayProviders/manage"));
                    break;
                case "reports":
                    collection.Add(CreateTreeNode("salesOverTime", "reports", queryStrings, "Sales Over Time", "icon-loading", false, "merchello/merchello/SalesOverTime/manage"));
                    collection.Add(CreateTreeNode("salesByItem", "reports", queryStrings, "Sales By Item", "icon-barcode", false, "merchello/merchello/SalesByItem/manage"));
                    collection.Add(CreateTreeNode("taxesByDestination", "reports", queryStrings, "Taxes By Destination", "icon-piggy-bank", false, "merchello/merchello/TaxesByDestination/manage"));
                    break;
                default:
                    collection.Add(CreateTreeNode("catalog", "", queryStrings, "Catalog", "icon-barcode", false, "merchello/merchello/ProductList/manage"));
                    collection.Add(CreateTreeNode("orders", "", queryStrings, "Orders", "icon-receipt-dollar", false, "merchello/merchello/OrderList/manage"));
                    collection.Add(CreateTreeNode("customers", "", queryStrings, "Customers", "icon-settings", false, "merchello/merchello/CustomerList/manage"));
                    collection.Add(CreateTreeNode("settings", "", queryStrings, "Settings", "icon-settings", true, "merchello/merchello/Settings/manage"));
                    break;
            }

            return collection;
        }

        /// <summary>
        /// The get menu for node.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="MenuItemCollection"/>.
        /// </returns>
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == "settings")
            {
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            }

            ////if (id == "catalog")
            ////{
            //    //create product
            ////    menu.Items.Add<MerchelloActionNewProduct>(ui.Text("actions", MerchelloActionNewProduct.Instance.Alias));
            ////}

            return menu;
        }

        private TreeNode GetTreeNodeFromConfigurationElement(TreeElement tree, FormDataCollection queryStrings, TreeElement parentTree = null)
        {
            return CreateTreeNode(
                tree.Id,
                parentTree == null ? string.Empty : parentTree.Id,
                queryStrings,
                tree.Title,
                tree.Icon,
                parentTree != null,
                tree.RoutePath);
        }
    }
}