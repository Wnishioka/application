using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System;

using System.Text;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;


public class Main : MonoBehaviour
{
    public string SERVER_ADDRESS = "http://202.143.198.111/game/";
    // public string SERVER_ADDRESS = "http://192.168.11.13/game/";

    [System.Serializable]
    private class SendDataJson
    {
        public string userName = default;
        public string questionNumber = default;
        public string question = default;
        public string correctAnswer = default;
        public string userAnswer = default;
        public string responseTime = default;
    }

    private SendDataJson sendData = new SendDataJson();

    [SerializeField] private Button operationButton = default;
    [SerializeField] private TextMeshProUGUI questionText = default;
    [SerializeField] private TextMeshProUGUI resultText = default;
    [SerializeField] private TMP_InputField answerInputField = default;

    private List<string[]> questions = default;
    private int questionCount = 0;
    private DateTime dt = default;

    enum Mode
    {
        Start,
        Answer,
        Next,
    }
    Mode mode = Mode.Start;

    private string GetUserName()
    {
        string userName = "";

        if (CheckWebGLPlatform())
        {
            userName = HttpCookie.GetCookie("user_name");
        }
        return userName;
    }

    protected bool CheckWebGLPlatform()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.sendData.userName = this.GetUserName();

        CSVManager csvManager = new CSVManager();
        this.questions = csvManager.CSVReadFromResource("questions");

        this.questionText.gameObject.SetActive(false);
        this.resultText.gameObject.SetActive(false);
        this.answerInputField.gameObject.SetActive(false);

        this.operationButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
    }

    public void OnClickButton(Button sender)
    {
        switch (sender.name)
        {
            case "OperationButton":

                switch (this.mode)
                {
                    case Mode.Start:

                        this.questionText.text = $"第{this.questions[this.questionCount][0]}問 : {this.questions[this.questionCount][1]}";

                        this.questionText.gameObject.SetActive(true);
                        this.resultText.gameObject.SetActive(true);
                        this.answerInputField.gameObject.SetActive(true);

                        this.operationButton.GetComponentInChildren<TextMeshProUGUI>().text = "Answer";
                        
                        this.dt = DateTime.Now;
                        this.mode = Mode.Answer;

                        break;

                    case Mode.Answer:

                        this.resultText.text = $"答えは{this.questions[this.questionCount][2]}です。";

                        this.sendData.questionNumber = this.questions[this.questionCount][0];
                        this.sendData.question = this.questions[this.questionCount][1];
                        this.sendData.correctAnswer = this.questions[this.questionCount][2];
                        this.sendData.userAnswer = this.answerInputField.text;

                        TimeSpan responceTime = DateTime.Now - this.dt;
                        this.sendData.responseTime = responceTime.ToString();

                        string jsonstr = JsonUtility.ToJson(this.sendData);
                        Debug.Log(jsonstr);

                        StartCoroutine(Post(SERVER_ADDRESS, jsonstr));

                        this.operationButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                        this.mode = Mode.Next;
                        
                        break;

                    case Mode.Next:

                        if (this.questionCount < this.questions.Count - 1)
                        {
                            this.resultText.text = "";
                            this.answerInputField.text = "";

                            this.questionCount += 1;
                            this.questionText.text = $"第{this.questions[this.questionCount][0]}問 : {this.questions[this.questionCount][1]}";

                            this.operationButton.GetComponentInChildren<TextMeshProUGUI>().text = "Answer";

                            this.dt = DateTime.Now;
                            this.mode = Mode.Answer;
                        }
                        else
                        {
                            this.questionText.gameObject.SetActive(false);
                            this.answerInputField.gameObject.SetActive(false);

                            this.resultText.text = "終了してください。";
                            this.operationButton.gameObject.SetActive(false);
                        }
                        break;

                    default:
                        break;
                }
                break;

            case "ExitButton":

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // ゲームプレイ終了
#else
                Application.Quit(); // ゲームプレイ終了
#endif
                break;

            default:
                break;
        }
    }

    IEnumerator Post(string url, string jsonstr)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);
    }
}
