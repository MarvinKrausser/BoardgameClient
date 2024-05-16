using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class DropZoneTest
    {
        [UnityTest]
        public IEnumerator OnDropTest()
        {
            var dropZoneObject = new GameObject();
            var dropZone = dropZoneObject.AddComponent<DropZone>();

      
            var dragableObject = new GameObject();
            var dragable = dragableObject.AddComponent<Dragable>();
            var parentToReturnTo = new GameObject().transform;

            var initialParent = new GameObject().transform;
            dragableObject.transform.SetParent(initialParent);

        
            var eventData = new PointerEventData(EventSystem.current);
            eventData.pointerDrag = dragableObject;
        
            dropZone.OnDrop(eventData);
        
            Assert.AreEqual(dropZoneObject.transform, dragable.transform.parent);
        
            GameObject.Destroy(dropZoneObject);
            GameObject.Destroy(dragableObject);
            GameObject.Destroy(initialParent.gameObject);
            GameObject.Destroy(parentToReturnTo.gameObject);
        
            yield return null;
        }
    }

}
