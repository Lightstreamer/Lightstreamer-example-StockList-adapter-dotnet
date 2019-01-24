#region License
/*
 * Copyright (c) Lightstreamer Srl
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion License

using System;
using System.Collections;
using System.Threading;

using Lightstreamer.Interfaces.Data;
using Lightstreamer.Adapters.StockListDemo.Feed;

namespace Lightstreamer.Adapters.StockListDemo.Data
{

    /// <summary>
    /// This Data Adapter accepts a limited set of item names (the names starting
    /// with "item") and listens to a (simulated) stock quotes feed, waiting for
    /// update events. The events pertaining to the currently subscribed items
    /// are then forwarded to Lightstreamer.
    /// This example demonstrates how a Data Adapter could interoperate with
    /// a broadcast feed, which sends data for all items. Many other types of feeds
    /// may exist, with very different behaviours.
    /// </summary>
    public class StockListDemoAdapter : IDataProvider, IExternalFeedListener
    {

        private IDictionary _subscribedItems;
        private ExternalFeedSimulator _myFeed;

        private IItemEventListener _listener;

        public StockListDemoAdapter()
        {
            _subscribedItems = new Hashtable();

            _myFeed = new ExternalFeedSimulator();
        }

        // ////////////////////////////////////////////////////////////////////////
        // IDataProvider methos

        public void Init(IDictionary parameters, string configFile)
        {
            _myFeed.SetFeedListener(this);
            _myFeed.Start();
        }

        public void SetListener(IItemEventListener eventListener)
        {
            _listener = eventListener;
        }

        public void Subscribe(string itemName)
        {
            if (!itemName.StartsWith("item"))
                throw new SubscriptionException("Unexpected item: " + itemName);

            lock (_subscribedItems)
            {
                if (_subscribedItems.Contains(itemName)) return;

                SubscriptionInfo si = new SubscriptionInfo(false, true);
                _subscribedItems[itemName] = si;
            }
            _myFeed.SendCurrentValues(itemName);
        }

        public void Unsubscribe(string itemName)
        {
            if (!itemName.StartsWith("item"))
                throw new SubscriptionException("Unexpected item: " + itemName);

            lock (_subscribedItems)
            {
                SubscriptionInfo si = (SubscriptionInfo)_subscribedItems[itemName];
                if (si != null)
                {
                    _subscribedItems.Remove(itemName);
                    lock (si)
                    {
                        si.stopSubscription();
                    }
                }
            }
        }

        public bool IsSnapshotAvailable(string itemName)
        {
            if (!itemName.StartsWith("item"))
                throw new SubscriptionException("Unexpected item: " + itemName);

            return true;
        }

        // never used in the demo, just showing the feature
        public void ClearStatus()
        {
            lock (_subscribedItems)
            {
                foreach (string itemName in _subscribedItems.Keys)
                {
                    _listener.ClearSnapshot(itemName);
                }
            }
        }

        // ////////////////////////////////////////////////////////////////////////
        // IExternalFeedListener methods

        public void OnEvent(string itemName,
            IDictionary currentValues,
            bool isSnapshot)
        {

            SubscriptionInfo si;
            lock (_subscribedItems)
            {
                if (!_subscribedItems.Contains(itemName)) return;

                si = (SubscriptionInfo)_subscribedItems[itemName];
            }

            lock (si)
            {
                bool started = si.getSnapshotReceived();
                if (!started)
                {
                    if (!isSnapshot)
                        return;

                    si.setSnapshotReceived();
                }
                else
                {
                    if (isSnapshot)
                    {
                        isSnapshot = false;
                    }
                }

                // We have to ensure that Update cannot be called after
                // Unsubscribe, so we need to hold the _subscribedItems lock;
                // however, Update is nonblocking; moreover, it only takes locks
                // to first order mutexes; so, it can safely be called here

                // Note that, in case a rapid Subscribe-Unsubscribe-Subscribe
                // sequence has just been issued for this item,
                // we may still be receiving and forwarding the snapshot
                // related with the first Subscribe call;
                // this case still leads to a perfectly consistent update flow,
                // in this scenario, so no checks are inserted to detect the case
                if (si.isStillSubscribed())
                {
                    _listener.Update(itemName, currentValues, isSnapshot);
                }
            }
        }

        // Manages the current state of the subscription 
        // for a single Item.
        private class SubscriptionInfo
        {
            bool snapshotReceived;
            public void setSnapshotReceived()
            {
                this.snapshotReceived = true;
            }

            public Boolean getSnapshotReceived()
            {
                return snapshotReceived;
            }

            bool stillSubscribed;

            public bool isStillSubscribed()
            {
                return this.stillSubscribed;
            }

            public SubscriptionInfo(bool snapshotReceived, bool isStillSubscribed)
            {
                this.snapshotReceived = snapshotReceived;
                this.stillSubscribed = isStillSubscribed;
            }

            public void stopSubscription()
            {
                this.stillSubscribed = false;
            }
        }
    }

}