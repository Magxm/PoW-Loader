using System;
using System.Collections.Generic;

using ModAPI.UI;

using UnityEngine;
using UnityEngine.EventSystems;

namespace ModAPI
{
    class InspectorScript : MonoBehaviour
    {
        /// <summary>
        /// Amount to scale UI by.
        /// </summary>
        public float scaleFactor = 1f;
        private GUIWindow Window = new GUIWindow("Inspector", 50, 50, 500, 500, 20, 40, 5);

        private Dictionary<Type, List<Type>> derivedTypes = new Dictionary<Type, List<Type>>();
        private string GetTypeString(GameObject obj)
        {
            string typeString = "";
            Queue<Type> toDo = new Queue<Type>();
            List<Type> result = new List<Type>();

            toDo.Enqueue(obj.GetType());
            while (toDo.Count > 0)
            {
                Type type = toDo.Dequeue();
                List<Type> dTypes = new List<Type>();
                if (derivedTypes.TryGetValue(type, out dTypes))
                {
                    foreach (Type t in dTypes)
                    {
                        if (!result.Contains(t))
                        {
                            toDo.Enqueue(t);
                            result.Add(t);
                        }
                    }
                }
            }

            foreach (Type t in result)
            {
                if (typeString != "") typeString += "=>";
                typeString += t.ToString();
            }

            return typeString;
        }

        private void DisplayCurrentlyPressedUINames()
        {
            DisplayCurrentlyPressedUINames(GetEventSystemRaycastResults());
        }

        private void DisplayCurrentlyPressedUINames(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                GameObject go = curRaysastResult.gameObject;
                if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                {

                    GUILayout.Label("Name:" + go.name + " | Type: " + GetTypeString(go));
                }
            }
        }

        ///Gets all event systen raycast results of current mouse or touch position.
        private List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

        private List<Type> FindAllDerivedTypes(Type baseType)
        {
            List<Type> resultTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyTypes = assembly.GetTypes();
                // Debug.Log(assembly.GetName() + ":");
                foreach (var type in assemblyTypes)
                {
                    //Debug.Log("   " + type.ToString());
                    if (type != baseType && baseType.IsAssignableFrom(type))
                    {
                        // Debug.Log("   " + type.ToString());
                        resultTypes.Add(baseType);
                    }
                }
            }

            return resultTypes;
        }

        public void Start()
        {
            //Initing type list which contains all needed derived types from GameObject
            Queue<Type> toDo = new Queue<Type>();
            toDo.Enqueue(typeof(GameObject));

            //Debug.Log("Start called!");
            while (toDo.Count > 0)
            {
                Type next = toDo.Dequeue();
                //Debug.Log("Checking " + next.ToString());
                List<Type> dTypes = FindAllDerivedTypes(next);
                derivedTypes.Add(next, dTypes);

                foreach (Type t in dTypes)
                {
                    //Debug.Log("Type: " + t.ToString());
                    if (!derivedTypes.ContainsKey(t))
                    {
                        toDo.Enqueue(t);
                    }
                }
            }

        }
        public void Update()
        {
        }

        private void OnDraw(int id)
        {
            GUILayout.Space(50);
            GUILayout.Label("Currently hovered UI Elements:");
            DisplayCurrentlyPressedUINames();

            Window.MakeDragable();
        }
        public void OnGUI()
        {
            Window.Render(OnDraw);
        }
    }
}
