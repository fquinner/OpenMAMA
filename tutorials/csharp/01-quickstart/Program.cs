﻿using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Wombat;


namespace _01_quickstart
{
    class Program
    {
        private static void usageAndExit() {
            Console.WriteLine("This is an OpenMAMA market data subscriber which assumes an");
            Console.WriteLine("OpenMAMA compatible data source is available to connect to and is");
            Console.WriteLine("configured in mama.properties.\n");
            Console.WriteLine("For more information, see http://openmama.org/quickstart.html\n");
            Console.WriteLine("Usage: dotnet run -s [symbol] [arguments]\n");
            Console.WriteLine("Arguments:");
            Console.WriteLine("\t-m [middleware]\tMiddleware bridge to load. Default: [qpid]");
            Console.WriteLine("\t-S [source]\tSource name (prefix) to use. Default: [OM]");
            Console.WriteLine("\t-t [transport]\tTransport from mama.properties to use. Default: [sub]");
            Console.WriteLine("\t-B\t\tDisables dictionary request");
            Console.WriteLine("\t-I\t\tPrevents an intial from being requested");
            System.Environment.Exit(1);
        }

        internal class SubscriptionEventHandler : MamaSubscriptionCallback
        {
            public void onMsg(MamaSubscription subscription, MamaMsg msg)
            {
                Console.WriteLine("Message Received: " + msg.ToString());
            }

            public void onError(MamaSubscription subscription, MamaStatus.mamaStatus status, string subject)
            {
                Console.WriteLine("An error occurred creating subscription for " + subject + ": " + status.ToString());
            }

            public void onCreate(MamaSubscription subscription)
            {
                // You may add your own event handling logic here
            }

            public void onQuality(MamaSubscription subscription, mamaQuality quality, string symbol)
            {
                // You may add your own event handling logic here
            }

            public void onDestroy(MamaSubscription subscription)
            {
                // You may add your own event handling logic here
            }

            public void onGap(MamaSubscription subscription)
            {
                // You may add your own event handling logic here
            }
            public void onRecapRequest(MamaSubscription subscription)
            {
                // You may add your own event handling logic here
            }
        }

        static void Main(string[] args)
        {
            // Parse the command line options to override defaults
            String middlewareName = "qpid";
            String transportName = "sub";
            String sourceName = "OM";
            String symbolName = null;
            String dictionaryFile = "/opt/openmama/data/dictionaries/data.dict";
            bool requiresDictionary = true;
            bool requiresInitial = true;

            if (args.Length == 0) {
                usageAndExit();
            }

            for (int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                case "-B":
                    requiresDictionary = false;
                    break;
                case "-d":
                    dictionaryFile = args[++i];
                    break;
                case "-I":
                    requiresInitial = false;
                    break;
                case "-m":
                    middlewareName = args[++i];
                    break;
                case "-s":
                    symbolName = args[++i];
                    break;
                case "-S":
                    sourceName = args[++i];
                    break;
                case "-t":
                    transportName = args[++i];
                    break;
                case "-v":
                    Mama.enableLogging (MamaLogLevel.MAMA_LOG_LEVEL_FINEST);
                    break;
                default:
                    usageAndExit();
                    break;
                }
            }

            // Symbol name is mandatory here, so error if it's null
            if (symbolName == null) {
                usageAndExit();
            }

            // Load the requested OpenMAMA middleware bridge (and default payload too)
            MamaBridge bridge = Mama.loadBridge(middlewareName);

            // Time to initialize OpenMAMA's underlying bridges with an "open"
            Mama.open();

            // Get default event queue from the bridge for testing
            MamaQueue queue = Mama.getDefaultEventQueue(bridge);

            // Set up the required transport on the specified bridge
            MamaTransport transport = new MamaTransport();
            transport.create(transportName, bridge);

            // Set up the data dictionary
            MamaDictionary dictionary = null;
            if (requiresDictionary) {
                dictionary = new MamaDictionary();
                dictionary.create(dictionaryFile);
            }

            // Set up the OpenMAMA source (symbol namespace)
            MamaSource source = new MamaSource();
            source.symbolNamespace = sourceName;
            source.transport = transport;

            // Set up the event handlers for OpenMAMA
            SubscriptionEventHandler eventHandler = new SubscriptionEventHandler();

            // Set up the OpenMAMA Subscription (interest in a topic)
            MamaSubscription subscription = new MamaSubscription();
            subscription.setRequiresInitial(requiresInitial);
            subscription.create(queue, eventHandler, source, symbolName);

            // Kick off OpenMAMA now (blocking call, non-blocking call also available)
            Mama.start(bridge);

            // Clean up connection on termination
            Mama.close();
        }
    }
}
