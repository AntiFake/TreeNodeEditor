using UnityEngine;
using System.Collections.Generic;

namespace QuestManagerEditor
{
    public class QuestManager : MonoBehaviour
    {
        //public List<TreeLink> questTree;

        public ComponentData data;

        public void Start()
        {
			if (data == null)
				Debug.Log ("ХУЙ!");
			else
				Debug.Log ("Та неужели");
        }

        public void Update()
        {
            
        }
    }
}