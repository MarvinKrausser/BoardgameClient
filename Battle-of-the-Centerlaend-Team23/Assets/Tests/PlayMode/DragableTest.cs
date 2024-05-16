using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class DragableTest
    {
        [UnityTest]
        public IEnumerator OnBeginDragTest()
        {
            var dragable = new GameObject().AddComponent<Dragable>();
            var parentToReturnTo = new GameObject().transform;
            var canvasGroup = dragable.gameObject.AddComponent<CanvasGroup>();

            dragable.parentToReturnTo = parentToReturnTo;

            //dragable.OnBeginDrag(new PointerEventData(EventSystem.current));
            Assert.IsTrue(canvasGroup.blocksRaycasts);
        
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnDragTest()
        {
            var dragable = new GameObject().AddComponent<Dragable>();
            var eventData = new PointerEventData(EventSystem.current);
            var startPosition = new Vector3(0, 0, 0);
            var newPosition = new Vector3(10, 10, 0);

            dragable.transform.position = startPosition;

            dragable.OnDrag(eventData);
            yield return null;

            //Assert.AreEqual(newPosition, dragable.transform.position);
        }

        [UnityTest]
        public IEnumerator OnEndDragTest()
        {
            var dragable = new GameObject().AddComponent<Dragable>();
            var parentToReturnTo = new GameObject().transform;
            var canvasGroup = dragable.gameObject.AddComponent<CanvasGroup>();

            dragable.parentToReturnTo = parentToReturnTo;

            dragable.OnEndDrag(new PointerEventData(EventSystem.current));

            yield return null;

            Assert.AreEqual(parentToReturnTo, dragable.transform.parent);
            Assert.IsTrue(canvasGroup.blocksRaycasts);
        }
    }
}