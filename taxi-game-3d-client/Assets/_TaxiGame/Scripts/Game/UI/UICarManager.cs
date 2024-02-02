using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class UICarManager : MonoBehaviour
    {
        Dictionary<string, GameObject> unused = new();
        
        public string SelectedId
        {
            get;
            private set;
        }
        public GameObject SelectedObject
        {
            get;
            private set;
        }

        public void Select(string id)
        {
            if (SelectedId == id)
                return;

            if (SelectedObject != null)
                Deselect();
            
            if (!unused.TryGetValue(id, out var go))
            {
                var template = ClientManager.Instance.TemplateService.Cars.Find(e => e.Id == id);
                go = Instantiate(template.UiPrefab, transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
            go.SetActive(true);
            SelectedId = id;
            SelectedObject = go;
        }

        public void Deselect()
        {
            if (SelectedObject == null)
                return;
            SelectedObject.SetActive(false);
            unused[SelectedId] = SelectedObject;
            SelectedObject = null;
            SelectedId = null;
        }
    }
}