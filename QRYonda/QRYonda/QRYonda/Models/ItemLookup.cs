/**********************************************************************************************
 * Copyright 2009 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file 
 * except in compliance with the License. A copy of the License is located at
 *
 *       http://aws.amazon.com/apache2.0/
 *
 * or in the "LICENSE.txt" file accompanying this file. This file is distributed on an "AS IS"
 * BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under the License. 
 *
 * ********************************************************************************************
 *
 *  Amazon Product Advertising API
 *  Signed Requests Sample Code
 *
 *  API Version: 2009-03-31
 *
 */
// This sample from https://aws.amazon.com/code/Product-Advertising-API/2480

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace QRYonda.Models
{
    public class ItemLookup
    {
        private const string MY_AWS_ASSOCIATE_TAG = "xxxxxxxx"; // Added from sample
        private const string MY_AWS_ACCESS_KEY_ID = "xxxxxxxx";
        private const string MY_AWS_SECRET_KEY = "xxxxxxxx";
        private const string DESTINATION = "webservices.amazon.co.jp";  // Changed from sample

        private const string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2011-08-01"; // Changed from sample
        private const string ID_TYPE = "ISBN"; // Added from sample
        private string ITEM_ID = "";

        public async Task<TodoItem> Lookup(string isbn)
        {
            if (isbn.Length == 13)
                this.ITEM_ID = isbn;
            else if (isbn.Length == 10)
                this.ITEM_ID = IsbnConverter.ConvertToISBN13(isbn);
            else
                //return "";
                return null;

            SignedRequestHelper helper = new SignedRequestHelper(
                MY_AWS_ASSOCIATE_TAG,
                MY_AWS_ACCESS_KEY_ID,
                MY_AWS_SECRET_KEY,
                DESTINATION,
                ID_TYPE);

            /*
             * The helper supports two forms of requests - dictionary form and query string form.
             */
            String requestUrl;
            //String itemInfo;
            TodoItem itemInfo;

            /*
             * Here is an ItemLookup example where the request is stored as a dictionary.
             */
            IDictionary<string, string> r1 = new Dictionary<string, String>();
            r1["Service"] = "AWSECommerceService";
            //r1["Version"] = "2009-03-31";
            r1["Operation"] = "ItemLookup";
            r1["ItemId"] = ITEM_ID;
            r1["ResponseGroup"] = "Images,Small";

            requestUrl = helper.Sign(r1);
            itemInfo = await FetchTitle(requestUrl);

            System.Diagnostics.Debug.WriteLine("ItemLookup Dictionary form.");
            System.Diagnostics.Debug.WriteLine($"ItemInfo is title: {itemInfo.Name}, image: {itemInfo.Image}");

            return itemInfo;
        }

        // Use XDocument indeed with XmlDocument.
        // https://forums.xamarin.com/discussion/comment/140386/#Comment_140386
        private static async Task<TodoItem> FetchTitle(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var reader = new StreamReader(await client.GetStreamAsync(url)))
                    {
                        var deserializer = new XmlSerializer(typeof(ItemLookupResponse));
                        var info = deserializer.Deserialize(reader) as ItemLookupResponse;

                        var itemTitle = info.Items.Item.ItemAttributes.Title;
                        var itemImageUrl = info.Items.Item.MediumImage.URL;
                        var itemUrl = info.Items.Item.DetailPageURL;

                        //return itemTitle;
                        return new TodoItem { Name = itemTitle, Image = itemImageUrl, Url = itemUrl};
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Caught Exception: " + e.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + e.StackTrace);
            }

            return null;
        }
    }
}

