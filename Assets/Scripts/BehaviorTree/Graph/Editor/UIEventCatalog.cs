using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;
using System.Linq;

namespace Benco.Graph
{
    // Might just change this whole thing into a dictionary and be done with it.
    public class UIEventCatalog
    {
        private class CatalogPage
        {
            public Type type;
            public List<UIEvent> list = new List<UIEvent>();
        }
        List<CatalogPage> catalog = new List<CatalogPage>();

        public void AddEntry(Type type, UIEvent uiEvent)
        {
            CatalogPage eventList = catalog.Find(x => x.type == type);
            if (eventList == null)
            {
                catalog.Add(new CatalogPage() { type = type, list = new List<UIEvent>() { uiEvent } });
            }
            else
            {
                eventList.list.Add(uiEvent);
            }
        }

        public void AddList(Type type, List<UIEvent> eventList)
        {
            CatalogPage page = catalog.Find(x => x.type == type);
            if (page == null)
            {
                catalog.Add(new CatalogPage() { type = type, list = eventList });
            }
            else
            {
                page.list.AddRange(eventList);
            }
        }
    }
}
