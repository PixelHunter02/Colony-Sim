using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace Highscores
{
    public class NewReadHighScores : MonoBehaviour
    {
        private Transform _hsContainer;
        private Transform _hsTemplate;
        private List<Transform> _hsLadderTransforms;

        public delegate void EntriesLoaded();
        public static event EntriesLoaded EntriesLoadedEvent;



        private void Awake()
        {
            
            if(SceneManager.GetActiveScene().name == "MainMenu")
            {
                // Get References
                _hsContainer = GameObject.Find("HighScoreContainer").GetComponent<Transform>();
                _hsTemplate = GameObject.Find("HighScoreTemplate").GetComponent<Transform>();
            
                // GameObject.Find("Leader Board").SetActive(false);
                _hsTemplate.gameObject.SetActive(false);

                _hsLadderTransforms = new List<Transform>();
            }
        }

        // private void OnEnable() {
        //     UIHandler.ClearHighscoresEvent += ClearHighscoreList;
        // }
        //
        // private void OnDisable() {
        //     UIHandler.ClearHighscoresEvent -= ClearHighscoreList;
        // }

        public void LoadEntriesMain()
        {
            string url = "https://colonysjourneyleaderboard.azurewebsites.net/api/GetLeaderboard?code=QXtsrH_T5LYjAR1XZLd1DU4nd1uEXgzX6ZlPi28MgzBxAzFuboLrxA==" ;
            WebRequest.GetText(url, (error) =>{
                Debug.Log(error);
            },
            (onSuccess) => {
                Debug.Log(onSuccess);
                Highscores highscores = JsonConvert.DeserializeObject<Highscores>(onSuccess);
                foreach (var hsEntry in highscores.highscoreEntryList)
                {
                    HighscoreEntryTransform(hsEntry, _hsContainer, _hsLadderTransforms);        
                }
                EntriesLoadedEvent?.Invoke();
            });
        }
        
        private void HighscoreEntryTransform(HighscoreEntries hsEntries, Transform container, List<Transform> hsLadderTransformList)
        {
            //set the positions of the ladders
            Transform hsLadderPosition = Instantiate(_hsTemplate, container);

            int rank = hsLadderTransformList.Count+1;

            hsLadderPosition.Find("Rank").GetComponent<TMP_Text>().text = rank.ToString();

            hsLadderPosition.Find("Score").GetComponent<TMP_Text>().text = hsEntries.score.ToString("f2");

            hsLadderPosition.Find("Name").GetComponent<TMP_Text>().text = hsEntries.name;


            hsLadderTransformList.Add(hsLadderPosition);
            hsLadderPosition.gameObject.SetActive(true);
        }
        
        private void ClearHighscoreList()
        {
            _hsLadderTransforms.Clear();
            foreach(GameObject entry in GameObject.FindGameObjectsWithTag("HighscoreEntry"))
            {
                Destroy(entry);
            }
        }

        public static void UploadEntry(string uri, int score, string name)
        {
            HighscoreEntries hsEntry = new HighscoreEntries{name = name, score = score};
            string jsonString = JsonConvert.SerializeObject(hsEntry);
            WebRequest.SendJsonData(uri, jsonString,(onError) => {
                    Debug.Log(onError);
                },(onSuccess) => {
                    Debug.Log(onSuccess);
                });
        }
       
        private class Highscores
        {
            public List<HighscoreEntries> highscoreEntryList;
        }

        [System.Serializable]
        private class HighscoreEntries
        {
            public int score;
            public string name;
        }
    }
    
}
