using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using System.Reflection;

namespace CustomShops.DEBUG_Patches
{
#if CCDEBUG
    [HarmonyPatch(typeof(ItemCollectionResultGenerator))]
    [HarmonyPatch("GenerateItemCollection")]
    public static class ItemCollectionResultGeneratore_GenerateItemCollection
    {
        [HarmonyPrefix]
        public static void OnICGenerate(ItemCollectionDef collection, int count, string parentGUID)
        {
            if (parentGUID == null)
                Control.LogDebug(DInfo.ItemGeneration, $"Generate {count} items of {collection?.ID} - root");
            else
                Control.LogDebug(DInfo.ItemGeneration, $"Generate {count} items of {collection?.ID} - {parentGUID}");

        }

        [HarmonyPostfix]
        public static void OnICGenerated(ItemCollectionDef collection, ItemCollectionResult __result)
        {
            if (__result != null)
                Control.LogDebug(DInfo.ItemGeneration, $"Generated {collection?.ID}. Completed");
            else
                Control.LogDebug(DInfo.ItemGeneration, $"Generated {collection?.ID}. Gather Dependences");
        }
    }


    [HarmonyPatch(typeof(ItemCollectionResultGenerator))]
    [HarmonyPatch(new Type[] { typeof(ItemCollectionDef.Entry), typeof(List<ShopDefItem>) })]
    [HarmonyPatch("InsertItemCollectionEntry")]
    public static class ItemCollectionResultGenerator_InsertItemCollectionEntry
    {
        [HarmonyPrefix]
        public static void OnInsert(ItemCollectionDef.Entry entry, List<ShopDefItem> items)
        {
            Control.LogDebug(DInfo.ItemGeneration, $"- inserting {entry.Count}x {entry.ID}, {entry.Type}");
        }
    }


    [HarmonyPatch(typeof(ItemCollectionResultGenerator))]
    [HarmonyPatch("OnCollectionComplete")]
    public static class ItemCollectionResultGenerator_OnCollectionComplete
    {
        [HarmonyPrefix]
        public static void OnComplete(ItemCollectionResult collectionResult)
        {
            Control.LogDebug(DInfo.ItemGeneration, $"Completed(prefix) {collectionResult.itemCollectionID} guid: {collectionResult.parentGUID}");
            Control.LogDebug(DInfo.ItemGeneration, $"- {collectionResult.items.Count} items {collectionResult.pendingCount} collections left");

        }

        [HarmonyPostfix]
        public static void OnCompleted(ItemCollectionResult collectionResult)
        {
            Control.LogDebug(DInfo.ItemGeneration, $"Completed(postfix) {collectionResult.itemCollectionID} guid: {collectionResult.parentGUID}");
            Control.LogDebug(DInfo.ItemGeneration, $"- {collectionResult.items.Count} items {collectionResult.pendingCount} collections left");
        }
    }

    [HarmonyPatch(typeof(ItemCollectionResultGenerator))]
    [HarmonyPatch("OnCollectionDefRetrieved")]
    public static class ItemCollectionResultGenerator_OnCollectionDefRetrieved
    {
        [HarmonyPrefix]
        public static void OnComplete(ref ItemCollectionDef def, Dictionary<string, List<KeyValuePair<ItemCollectionDef.Entry, string>>> ____pendingGenerateRequests)
        {
            if (def != null)
                Control.LogDebug(DInfo.ItemGeneration, $"Retrieved  {def.ID}");
            else
            {
                String error = "Retrived empty collection! Pending Collections:\n";

                if (____pendingGenerateRequests != null)
                {
                    foreach (var pair in ____pendingGenerateRequests)
                    {
                        error += "- " + pair.Key + "\n"; 
                        foreach (var pair2 in pair.Value)
                        {
                            error += "- " + pair2.Key.ID + "\n";
                        }
                    }
                }
                else
                {
                    error += "- cannot get pending requests\n";
                }
                //error += "replacing with DUMMY";
                //def = Control.State.Sim.DataManager.ItemCollectionDefs.Get("ItemCollection_DUMMY");
                Control.LogError(error);
            }
        }
    }


    [HarmonyPatch(typeof(ItemCollectionResultGenerator))]
    [HarmonyPatch("ProcessQueuedReferenceCollections")]
    public static class ItemCollectionResultGenerator_ProcessQueuedReferenceCollections
    {
        public static bool Request = false;

        [HarmonyPrefix]
        public static void OnComplete(List<ItemCollectionDef.Entry> queuedReferences, ItemCollectionResult result)
        {
            Control.LogDebug(DInfo.ItemGeneration, $"ProcessQueuedReferenceCollections(prefix) {result.itemCollectionID} guid: {result.GUID} pending:{queuedReferences.Count}");
            Request = true;
        }

        [HarmonyPrefix]
        public static void OnCompleted()
        {
            Request = false;
        }
    }

    //[HarmonyPatch(typeof(SimGameState))]
    //public static class SimGameState_RequestItem
    //{
    //    public static bool Request = false;

    //    public static MethodInfo TargetMethod()
    //    {
    //        return typeof(SimGameState).GetMethod("RequestItem").MakeGenericMethod(typeof(ItemCollectionDef));
    //    }

    //    [HarmonyPrefix]
    //    public static void OnComplete(string id)
    //    {
    //        if(ItemCollectionResultGenerator_ProcessQueuedReferenceCollections.Request)
    //        Control.LogDebug(DInfo.ItemGeneration, $"- ItemCollectionRequested: {id}");
    //    }
    //}



    [HarmonyPatch(typeof(Shop))]
    [HarmonyPatch("OnItemsCollected")]
    public static class Shop_OnItemsCollected
    {
        [HarmonyPrefix]
        public static void OnResultReveived(ItemCollectionResult result)
        {
            Control.LogDebug(DInfo.ItemGeneration, $"-- received result from {result.itemCollectionID}");
        }
    }

#endif
}
