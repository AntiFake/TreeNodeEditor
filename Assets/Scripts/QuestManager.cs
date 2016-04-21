using UnityEngine;
using System.Collections;

namespace QuestManagerEditor
{
    public class QuestManager : MonoBehaviour
    {
        public GameObject cam;
        public int health;
        public string username;

        public void Start()
        {
            Debug.Log(health);
            Debug.Log(cam.name);
            Debug.Log(username);
        }

        public void Update()
        {
            
        }
    }
}