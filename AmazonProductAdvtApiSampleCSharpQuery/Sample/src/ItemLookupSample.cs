﻿/**********************************************************************************************
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Serialization;

namespace AmazonProductAdvtApi
{
    class ItemLookupSample
    {
        private const string MY_AWS_ASSOCIATE_TAG = "xxxxx"; // Add.
        private const string MY_AWS_ACCESS_KEY_ID = "xxxxx";
        private const string MY_AWS_SECRET_KEY = "xxxxx";
        private const string DESTINATION = "webservices.amazon.co.jp";  // Change to Japan.

        private const string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2011-08-01"; // Change to latest.
        private const string ID_TYPE = "ISBN"; // Add.
        private const string ITEM_ID = "9784822298616";

        public static void Main()
        {
            SignedRequestHelper helper = new SignedRequestHelper(
                MY_AWS_ASSOCIATE_TAG, // Add.
                MY_AWS_ACCESS_KEY_ID,
                MY_AWS_SECRET_KEY,
                DESTINATION,
                ID_TYPE // Add.
            ); 
            
            /*
             * The helper supports two forms of requests - dictionary form and query string form.
             */
            String requestUrl;
            String title;

            /*
             * Here is an ItemLookup example where the request is stored as a dictionary.
             */
            IDictionary<string, string> r1 = new Dictionary<string, String>();
            r1["Service"] = "AWSECommerceService";
            r1["Version"] = "22011-08-01"; // Change to latest.
            r1["Operation"] = "ItemLookup";
            r1["ItemId"] = ITEM_ID;
            r1["ResponseGroup"] = "Images,Small"; // Change to get image url.

            /* Random params for testing */
            //r1["AnUrl"] = "http://www.amazon.com/books";
            //r1["AnEmailAddress"] = "foobar@nowhere.com";
            //r1["AUnicodeString"] = "αβγδεٵٶٷٸٹٺチャーハン叉焼";
            //r1["Latin1Chars"] = "ĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳ";

            requestUrl = helper.Sign(r1);
            // 仕方ないので。
            var task = FetchTitle(requestUrl);
            task.Wait();
            title = task.Result;

            System.Console.WriteLine("Method 1: ItemLookup Dictionary form.");
            System.Console.WriteLine("Title is \"" + title + "\"");
            System.Console.WriteLine();

            ///*
            // * Here is a CartCreate example where the request is stored as a dictionary.
            // */
            //IDictionary<string, string> r2 = new Dictionary<string, String>();
            //r2["Service"] = "AWSECommerceService";
            //r2["Version"] = "2009-03-31";
            //r2["Operation"] = "CartCreate";
            //r2["Item.1.OfferListingId"] = "Ho46Hryi78b4j6Qa4HdSDD0Jhan4MILFeRSa9mK+6ZTpeCBiw0mqMjOG7ZsrzvjqUdVqvwVp237ZWaoLqzY11w==";
            //r2["Item.1.Quantity"] = "1";

            //requestUrl = helper.Sign(r2);
            //title = FetchTitle(requestUrl);

            //System.Console.WriteLine("Method 1: CartCreate Dictionary form.");
            //System.Console.WriteLine("Cart Item Title is \"" + title + "\"");
            //System.Console.WriteLine();

            ///*
            // * Here is an example where the request is stored as a query-string:
            // */

            ///*
            // * string requestString = "Service=AWSECommerceService&Version=2009-03-31&Operation=ItemLookup&ResponseGroup=Small&ItemId=" + ITEM_ID;
            // */
            //System.Console.WriteLine("Method 2: Query String form.");

            //String[] Keywords = new String[] {
            //    "surprise!",
            //    "café",
            //    "black~berry",
            //    "James (Jim) Collins",
            //    "münchen",
            //    "harry potter (paperback)",
            //    "black*berry",
            //    "finger lickin' good",
            //    "!\"#$%'()*+,-./:;<=>?@[\\]^_`{|}~",
            //    "αβγδε",
            //    "ٵٶٷٸٹٺ",
            //    "チャーハン",
            //    "叉焼",
            //};

            //foreach (String keyword in Keywords)
            //{
            //    String requestString = "Service=AWSECommerceService" 
            //        + "&Version=2009-03-31"
            //        + "&Operation=ItemSearch"
            //        + "&SearchIndex=Books"
            //        + "&ResponseGroup=Small"
            //        + "&Keywords=" + keyword
            //        ;
            //    requestUrl = helper.Sign(requestString);
            //    title = FetchTitle(requestUrl);

            //    System.Console.WriteLine("Keyword=\"" + keyword + "\"; Title=\"" + title + "\"");
            //    System.Console.WriteLine();
            //}

            //String cartCreateRequestString = 
            //    "Service=AWSECommerceService"
            //    + "&Version=2009-03-31"
            //    + "&Operation=CartCreate"
            //    + "&Item.1.OfferListingId=Ho46Hryi78b4j6Qa4HdSDD0Jhan4MILFeRSa9mK%2B6ZTpeCBiw0mqMjOG7ZsrzvjqUdVqvwVp237ZWaoLqzY11w%3D%3D"
            //    + "&Item.1.Quantity=1"
            //    ;
            //requestUrl = helper.Sign(cartCreateRequestString);
            //title = FetchTitle(requestUrl);

            //System.Console.WriteLine("Cart Item Title=\"" + title + "\"");
            //System.Console.WriteLine();


            //System.Console.WriteLine("Hit Enter to end");
            //System.Console.ReadLine();
        }

        private static async Task<string> FetchTitle(string url)
        {
            try
            {
                // HttpClientでXMLを一気に取得する方法に変更
                using (var client = new HttpClient())
                {
                    using (var reader = new StreamReader(await client.GetStreamAsync(url)))
                    {
                        var deserializer = new XmlSerializer(typeof(ItemLookupResponse));
                        var info = deserializer.Deserialize(reader) as ItemLookupResponse;

                        var itemTitle = info.Items.Item.ItemAttributes.Title;
                        var itemImageUrl = info.Items.Item.MediumImage.URL; // ついでに
                        var itemUrl = info.Items.Item.DetailPageURL; // ついでに

                        return itemTitle;
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Caught Exception: " + e.Message);
                System.Console.WriteLine("Stack Trace: " + e.StackTrace);
            }

            return null;
        }
    }
}
