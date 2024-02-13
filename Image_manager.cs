using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Image_manager : MonoBehaviour
{
    private int REASON = 0;
    private int MONTH = 1;
    private int DATE = 2;
    private int SIGN = 3;
    private int ID = 4;
    private int LANGUAGE = 5;

    private int FONT_COUNT = 3;
    private int maxQuestionpattern = 21;
    private int QUESTION_PATTERN_1 = 20;
    private int QUESTION_PATTERN_2 = 7;
    private int QUESTION_PATTERN_3 = 21;
    private int NUMBER_OF_QUESTION_1 = 20; //問題1の問題数
    private int NUMBER_OF_QUESTION_2 = 7; //問題2の問題数
    private int NUMBER_OF_QUESTION_3 = 21; //問題3の問題数
    private int NUMBER_OF_ANSWER = 4;
    private int NUMBER_OF_DATAIMAGES = 7;

    // 192.244.210.74
    public const string SERVER_ADDRESS = "http://nihongo-ud.shimane-u.ac.jp/font/record";
    //private StreamWriter sw;

    // 追加部分
    // ##########
    private int GetUserID()
    {
        int userID = 0;

        if (CheckWebGLPlatform())
        {
            userID = int.Parse(HttpCookie.GetCookie("user_id"));
        }
        return userID;
    }

    protected bool CheckWebGLPlatform()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }
    // ##########


    [System.Serializable]
    private class SendDataConsent
    {
        public int userID = default;  // 追加
        //同意画面のログ
        public string type = default;
        public string userChoiceConsent = default;
        public string userDetail = default;
        public string month = default;
        public string date = default;
        public string userName = default;
    }
    private class SendDataDataPaper
    {
        public int userID = default;  // 追加
        public string userName = default; // 追加
        //個人データ入力のログ
        public string type = default;
        public string userId = default;
        public string userLanguage = default;
        public string userChoiceStudy = default;
        public List<string> userChoiceHowLearn = new List<string>();
        public string userChoiceJLPT = default;
        public string age = default;
    }
    private class SendDataQuestion
    {
        public int userID = default;  // 追加
        public string userName = default;  // 追加

        //問題のログ
        public string type = default;
        public int questionNumber = default;
        public int number = default;
        public string question = default;
        public List<string> answerText = new List<string>();
        public string userAnswer = default;
        public string font = default;
        public string judge = default;
        public float responseTime = default;
        public int backCount = default;
        public float backTime = default;
    }

    private SendDataConsent Consent = new SendDataConsent();
    private SendDataDataPaper DataPaper = new SendDataDataPaper();
    private SendDataQuestion Question = new SendDataQuestion();

    private float TITLE_WIDTH;
    private float TITLE_HEIGHT;
    private float QUESTION_WIDTH;
    private float QUESTION_HEIGHT;
    private float CONSENT_WIDTH;
    private float CONSENT_HEIGHT;
    private float DATA_WIDTH;
    private float DATA_HEIGHT;
    private float QUESTION3_WIDTH;
    private float QUESTION3_HEIGHT;
    private int toggle_flag;
    private int toggle_present;
    private int toggle_previous;
    private bool back_flag = false;
    private bool flag;

    //ゲームモード
    enum Mode
    {
        Consent_Main,
        Data,
        Title,
        Question1,
        Question2,
        Question3,
        Next,
    }
    private int title_number = 0;
    Mode _mode = Mode.Title;

    private Mode mode
    {
        get { return _mode; }
        set
        {
            _mode = value;
            CanvasManager();
        }
    }

    enum Selects
    {
        Num,
        Japan,
        Nihon,
        English,
        China,
        Beto,
        Myan,
    }
    Selects selects;
    Selects Language = Selects.Japan;

    //Itemクラス
    private Item[] ReadFile_Item;
    private Item_Question2_Text[] ReadFile_Item_Question2_Text;
    private Item_Question2_Answer[] ReadFile_Item_Question2_Answer;
    private Item_Question3_Text[] ReadFile_Item_Question3_Text;


    //各問題テキスト、イメージの保存配列
    private string[] Input_text = new string[100];
    private string[,] userChoice_data = new string[4, 6];
    private string[,] Csv_answers_1 = new string[100,100];
    private string[,] Csv_answers_2 = new string[100, 100];
    private int[,,] csv_answers_num1 = new int[100, 100,100];
    private int[,,] csv_answers_num2 = new int[100, 100,100];
    private Question_Text[] question_Text = new Question_Text[100];
    private string[] Question3_Text = new string[100];
    private string[] question3Answer = new string[100];
    //ローカル関数
    private int[,] Question_number_1 = new int[100,100];
    private int[,] Question_number_2= new int[100,100];
    private int[,] Question_number_3 = new int[100, 100];

    private int[,] FontArray = new int[100, 100];

    private float responce = 0;
    private int font_number = 0;
    private int round = 0;
    private int BackCount = 0;
    public int image_number = 0;
    public int main_number = 0;
    public int language_number = 0;
    public int csv_number = 0;
    public int answers_number = 0;
    //int con_flag = 0;

    float X, Y, Z;
    float X2, Y2, Z2;

    TimeSpan ResponceTime = default;
    TimeSpan Num = default;
    TimeSpan backTime = default;
    //private DateTime num = default;
    private DateTime dt = default;
    private DateTime back_dt = default;
    //private DateTime BackTime = default;


    public List<string[]> csvDatas = new List<string[]>();
    List<Item> list_item = new List<Item>();
    List<Item_Question2_Text> list_question2_text = new List<Item_Question2_Text>();
    List<Item_Question2_Answer> list_question2_answer = new List<Item_Question2_Answer>();
    List<Item_Question3_Text> list_question3_text = new List<Item_Question3_Text>();
    Item this_item;
    Item_Question2_Text this_question2_text;
    Item_Question2_Answer this_question2_answer;
    Item_Question3_Text this_question3_text;

    [SerializeField] Sprite[] sprites; //同意画面の画像


    [SerializeField] Sprite[] data_images; //個人データ入力画面の画像
    [SerializeField] Sprite[] title_images;  //各問題のタイトル画像
    [SerializeField] Sprite[] first_images;  //問題1の画像
    [SerializeField] Sprite[] firstQuestion; //問題1の選択肢
    [SerializeField] Sprite[] firtstQuestionOther;
    [SerializeField] Sprite[] second_images; //問題2の問題文の画像
    [SerializeField] Sprite[] secondQuestion; //問題2の選択肢
    [SerializeField] Sprite[] third_images;  //問題3の画像
    [SerializeField] Sprite[] thirdQuestion;  //問題3の選択肢

    //フォント
    public TMP_FontAsset biz;
    public TMP_FontAsset Msgothic;
    public TMP_FontAsset Udd;
    public TMP_FontAsset Yumin;

    Text textComp;


    // IuputField
    private TMP_InputField sign;
    private TMP_InputField reason;
    private TMP_InputField month;
    private TMP_InputField date;
    private TMP_InputField id;
    private TMP_InputField language;

    //image用のフィールド
    public Image consent_image;
    public Image data_image;
    public Image material1;
    public Image material2;
    public Image material3;
    public Image material4;
    public Image material5; 
    public Image material6;
    public Image material7;
    public Image material8;
    public Image material9;
    public Image material10;
    public Image material11;

    public Image material_other;


    public Image con_material1;
    public Image con_material2;
    public Image con_material3;
    public Image con_material4;
    public Image con_material5;
    public Image con_material6;
    public Image understandMate;
    public Image con_material8;
    public Image con_material9;
    public Image con_material10;
    public Image con_material11;
    public Image con_material12;

    public Image con_mateJapan_agree;
    public Image con_mateNihon_agree;
    public Image con_mateEnglish_agree;
    public Image con_mateChina_agree;
    public Image con_mateBeto_agree;
    public Image con_mateMyan_agree;

    public Image con_mateJapan_disagree;
    public Image con_mateNihon_disagree;
    public Image con_mateEnglish_disagree;
    public Image con_mateChina_disagree;
    public Image con_mateBeto_disagree;
    public Image con_mateMyan_disagree;

    public Image con_mateJapan_etc;
    public Image con_mateNihon_etc;
    public Image con_mateEnglish_etc;
    public Image con_mateChina_etc;
    public Image con_mateBeto_etc;
    public Image con_mateMyan_etc;

    public Image ageYes_material;
    public Image ageNo_material;
    public Image consentYes_material;
    public Image consentNo_material;

    public Image con_manage1;
    public Image con_manage2;
    public Image con_manage3;
    public Image con_manage4;
    public Image con_manage5;
    public Image con_manage6;


    public Image understandManage;

    public Image con_maneJapan_agree;
    public Image con_maneNihon_agree;
    public Image con_maneEnglish_agree;
    public Image con_maneChina_agree;
    public Image con_maneBeto_agree;
    public Image con_maneMyan_agree;

    public Image con_maneJapan_disagree;
    public Image con_maneNihon_disagree;
    public Image con_maneEnglish_disagree;
    public Image con_maneChina_disagree;
    public Image con_maneBeto_disagree;
    public Image con_maneMyan_disagree;

    public Image con_maneJapan_etc;
    public Image con_maneNihon_etc;
    public Image con_maneEnglish_etc;
    public Image con_maneChina_etc;
    public Image con_maneBeto_etc;
    public Image con_maneMyan_etc;

    public Image ageYes_mane;
    public Image ageNo_mane;
    public Image consentYes_mane;
    public Image consentNo_mane;

    public Image data_material1;
    public Image data_material2;
    public Image data_material3;
    public Image data_material4;
    public Image data_material5;
    public Image data_material6;
    public Image data_material7;
    public Image data_material8;
    public Image data_material9;
    public Image data_material10;
    public Image data_material11;
    public Image data_material12;
    public Image data_material13;
    public Image data_material14;
    public Image data_material15;
    public Image data_material16;
    public Image data_material17;
    public Image data_material18;
    public Image data_material19;
    public Image data_material20;
    public Image data_material21;

    public Image data_material_Language1;
    public Image data_material_Language2;
    public Image data_material_Language3;
    public Image data_material_Language4;
    public Image data_material_Language5;
    public Image data_material_Language6;
    public Image data_material_Language7;
    public Image data_material_Language8;
    public Image data_material_Language9;
    public Image data_material_Language10;
    public Image data_material_Language11;
    public Image data_material_Language12;
    public Image data_material_Language13;
    public Image data_material_Language14;
    public Image data_material_Language15;
    public Image data_material_Language16;

    public Image data_manage1;
    public Image data_manage2;
    public Image data_manage3;
    public Image data_manage4;
    public Image data_manage5;
    public Image data_manage6;
    public Image data_manage7;
    public Image data_manage8;
    public Image data_manage9;
    public Image data_manage10;
    public Image data_manage11;
    public Image data_manage12;
    public Image data_manage13;
    public Image data_manage14;
    public Image data_manage15;
    public Image data_manage16;
    public Image data_manage17;
    public Image data_manage18;
    public Image data_manage19;
    public Image data_manage20;
    public Image data_manage21;

    public Image data_manage_Language1;
    public Image data_manage_Language2;
    public Image data_manage_Language3;
    public Image data_manage_Language4;
    public Image data_manage_Language5;
    public Image data_manage_Language6;
    public Image data_manage_Language7; 
    public Image data_manage_Language8;
    public Image data_manage_Language9;
    public Image data_manage_Language10;
    public Image data_manage_Language11;
    public Image data_manage_Language12;
    public Image data_manage_Language13;
    public Image data_manage_Language14;
    public Image data_manage_Language15;
    public Image data_manage_Language16;

    public Image title_image;
    public Image question_image;

    public Image questionText_1_1;
    public Image questionText_1_2;
    public Image questionText_1_3;
    public Image questionText_1_4;
    public Image questionText_other;

    public Image yesImage;
    public Image noImage;
    public Image etcImage;

    public Image questionImage;
    public Image englishImage;
    public Image questionText_2_1;
    public Image questionText_2_2;
    public Image questionText_2_3;
    public Image questionText_2_4;

    public Image questionImage3;

    //Answer、Text用のフィールド
    public TextMeshProUGUI answer1;
    public TextMeshProUGUI answer2;
    public TextMeshProUGUI answer3;
    public TextMeshProUGUI answer4;
    private TextMeshProUGUI question2_japan;
    private TextMeshProUGUI question2_english;
    private TextMeshProUGUI answer5;
    private TextMeshProUGUI answer6;
    private TextMeshProUGUI answer7;
    private TextMeshProUGUI answer8;
    private TextMeshProUGUI question3_text;
    private RubyTextMeshProUGUI question3;

    //キャンバス
    public Canvas consent;  //同意画面のキャンバス
    private Canvas data_canvas; //データ入力キャンバス
    public Canvas title;  //問題のタイトル画面のキャンバス
    public Canvas canvas1;  //問題1のキャンバス
    public Canvas canvas2; //問題2のキャンバス
    public Canvas canvas3; //問題3のキャンバス

    //ボタン
    //各問題のボタン
    private Button Consent_Next;
    private Button Consent_Back;

    private Button Data_Next;
    private Button Data_back;
    public Button Question1_Next;
    private Button Question2_Next;
    private Button Question3_Next;
    //public  Button circle;
    //public  Button cross;
    public Button Question1_Back;
    private Button Question2_Back;
    private  Button Question3_Back;
    public Button Question1_Change;
    private Button Question2_Change;
    private Button End_Button;
    //タイトルチェンジボタン
    private Button Title_Change_Button;
    private Button Consent_Change_Button;
    private Button Data_Change_Button;
    //private Button Reset_Button;

    //タイトルに戻るボタン
    private Button backTitle_Button;
    private Button backTitle_Button2;

    //各問題のトグルスイッチ
    private Toggle con_toggle1;
    private Toggle con_toggle2;
    private Toggle con_toggle3;
    private Toggle con_toggle4;
    private Toggle con_toggle5;
    private Toggle con_toggle6;
    private Toggle understandToggle;

    private Toggle japan_agree;

    private Toggle japan_disagree;
    
    private Toggle japan_etc;

    private Toggle ageYes;
    private Toggle ageNo;
    private Toggle consentYes;
    private Toggle consentNo;

    private Toggle data_toggle1;
    private Toggle data_toggle2;
    private Toggle data_toggle3;
    private Toggle data_toggle4;
    private Toggle data_toggle5;
    private Toggle data_toggle6;
    private Toggle data_toggle7;
    private Toggle data_toggle8;
    private Toggle data_toggle9;
    private Toggle data_toggle10;
    private Toggle data_toggle11;
    private Toggle data_toggle12;
    private Toggle data_toggle13;
    private Toggle data_toggle14;
    private Toggle data_toggle15;
    private Toggle data_toggle16;
    private Toggle data_toggle17;
    private Toggle data_toggle18;
    private Toggle data_toggle19;
    private Toggle data_toggle20;
    private Toggle data_toggle21;

    private Toggle data_toggle_language1;
    private Toggle data_toggle_language2;
    private Toggle data_toggle_language3;
    private Toggle data_toggle_language4;
    private Toggle data_toggle_language5;
    private Toggle data_toggle_language6;
    private Toggle data_toggle_language7;
    private Toggle data_toggle_language8;
    private Toggle data_toggle_language9;
    private Toggle data_toggle_language10;
    private Toggle data_toggle_language11;
    private Toggle data_toggle_language12;
    private Toggle data_toggle_language13;
    private Toggle data_toggle_language14;
    private Toggle data_toggle_language15;
    private Toggle data_toggle_language16;

    private Toggle toggle1;
    private Toggle toggle2;
    private Toggle toggle3;
    private Toggle toggle4;
    private Toggle toggle_other;
    private Toggle toggle5;
    private Toggle toggle6;
    private Toggle toggle7;
    private Toggle toggle8;

    private Toggle yes;
    private Toggle no;
    private Toggle etc;


    // アスペクト比維持関数(Qustion1)
    public void Set_TextImage_Question1(Sprite set_sprite)
    {
        Sprite textimage_sprite = set_sprite;
        RectTransform rect = question_image.GetComponent<RectTransform>();

        float sprite_aspect = Get_AspectRate(textimage_sprite.rect.width, textimage_sprite.rect.height);
        if (sprite_aspect >= 1)
        {
            float width = QUESTION_WIDTH;
            float height = (width / textimage_sprite.rect.width) * textimage_sprite.rect.height;
            question_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        else
        {
            float height = QUESTION_HEIGHT;
            float width = (height / textimage_sprite.rect.height) * textimage_sprite.rect.width;
            question_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
    }

    // アスペクト維持関数(Title)
    public void Set_TextImage_Title(Sprite set_sprite)
    {
        Sprite textimage_sprite = set_sprite;
        RectTransform rect = title_image.GetComponent<RectTransform>();

        float sprite_aspect = Get_AspectRate(textimage_sprite.rect.width, textimage_sprite.rect.height);
        var titlePos = new Vector3(0, -300, 0);
        Title_Change_Button.transform.localPosition = titlePos;
        if (sprite_aspect >= 1)
        {
            float width = TITLE_WIDTH;
            float height = (width / textimage_sprite.rect.width) * textimage_sprite.rect.height;
            title_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        else
        {
            float height = TITLE_HEIGHT;
            float width = (height / textimage_sprite.rect.height) * textimage_sprite.rect.width;
            title_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        if(title_number == 2)
        {
            float w = TITLE_WIDTH;
            float h = TITLE_WIDTH * (textimage_sprite.rect.height / textimage_sprite.rect.width);
            title_image.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);

            titlePos = new Vector3(0, -550, 0);
            Title_Change_Button.transform.localPosition = titlePos;
        }
        else if(title_number == 4)
        {
            titlePos = new Vector3(0, -550, 0);
            Title_Change_Button.transform.localPosition = titlePos;
        }
        else if(title_number == 6)
        {
            titlePos = new Vector3(0, -300, 0);
            Title_Change_Button.transform.localPosition = titlePos;
        }
    }

    // アスペクト維持関数(Consent)
    public void Set_TextImage_Consent(Sprite set_sprite)
    {
        Sprite textimage_sprite = set_sprite;
        RectTransform rect = consent_image.GetComponent<RectTransform>();

        float width = CONSENT_WIDTH;
        float height = CONSENT_WIDTH * textimage_sprite.rect.height / textimage_sprite.rect.width;
        consent_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        if (selects == Selects.Num)
        {
            if (image_number == 1)
            {
                float w = CONSENT_WIDTH * 593 / 720;
                float h = CONSENT_WIDTH * 800 / 720;
                consent_image.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
            }
            else if (image_number == 7)
            {
                Vector3 consentPos = new Vector3(0, 0, 0);
                consent_image.transform.localPosition = consentPos;
            }
            else if (image_number == 8)
            {
                Vector3 consentPos = new Vector3(0, 0, 0);
                consent_image.transform.localPosition = consentPos;
            }
            else if (image_number == 10)
            {
                Vector3 consentPos = new Vector3(0, 0, 0);
                consent_image.transform.localPosition = consentPos;
            }
        }
        else if (selects != Selects.Num)
        {
            Vector3 consentPos = new Vector3(0, 0, 0);
            consent_image.transform.localPosition = consentPos;
            if (language_number == 1)
            {
                if (selects == Selects.Beto)
                {
                    float w = CONSENT_WIDTH * 550 / 720;
                    float h = CONSENT_WIDTH * 550 / 720 * (textimage_sprite.rect.height / textimage_sprite.rect.width);
                    consent_image.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);

                    consentPos = new Vector3(0, 20, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.Myan)
                {
                    if (language_number == 1)
                    {
                        float w = CONSENT_WIDTH * 550 / 720;
                        float h = CONSENT_WIDTH * 550 / 720 * (textimage_sprite.rect.height / textimage_sprite.rect.width);
                        consent_image.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);

                        consentPos = new Vector3(0, 20, 0);
                        consent_image.transform.localPosition = consentPos;
                    }
                }
            }
            else if (language_number == 2)
            {
                if (selects == Selects.Japan)
                {
                    consentPos = new Vector3(70, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.Nihon)
                {
                    consentPos = new Vector3(90, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.English)
                {
                    consentPos = new Vector3(80, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.China)
                {
                    consentPos = new Vector3(90, 5, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.Beto)
                {
                    consentPos = new Vector3(100, -10, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.Myan)
                {
                    consentPos = new Vector3(100, -10, 0);
                    consent_image.transform.localPosition = consentPos;
                }
            }
            else if (language_number == 3)
            {
                if (image_number != 8)
                {
                    if (selects == Selects.Japan)
                    {
                        consentPos = new Vector3(70, 10, 0);
                        consent_image.transform.localPosition = consentPos;
                    }
                    else if (selects == Selects.Nihon)
                    {
                        consentPos = new Vector3(40, 10, 0);
                        consent_image.transform.localPosition = consentPos;
                    }
                    else if (selects == Selects.Beto)
                    {
                        consentPos = new Vector3(0, 33, 0);
                        consent_image.transform.localPosition = consentPos;

                    }
                }
            }
            else if(language_number == 4 || language_number == 5)
            {
                if(selects == Selects.Japan || selects == Selects.Nihon)
                {
                    consentPos = new Vector3(90,0,0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if(selects == Selects.English)
                {
                    consentPos = new Vector3(10, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.China)
                {
                    consentPos = new Vector3(210, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.Beto)
                {
                    consentPos = new Vector3(50, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                }
                else if (selects == Selects.Myan)
                {
                    consentPos = new Vector3(20, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                }
            }
            else if (language_number == 7)
            {
                if (selects == Selects.Japan)
                {
                    consentPos = new Vector3(0, 15, 0);
                    consent_image.transform.localPosition = consentPos;
                }
            }
        }
    }
    // アスペクト維持関数(Data)
    public void Set_TextImage_Data(Sprite set_sprite)
    {
        Sprite textimage_sprite = set_sprite;
        RectTransform rect = data_image.GetComponent<RectTransform>();

        float width = DATA_WIDTH;
        float height = DATA_HEIGHT * textimage_sprite.rect.height / textimage_sprite.rect.width;
        data_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        if (image_number == 0)
        {
            if (Language == Selects.Japan)
            {
                var dataPos = new Vector3(220, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Nihon)
            {
                var dataPos = new Vector3(210, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.English)
            {
                var dataPos = new Vector3(100, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.China)
            {
                var dataPos = new Vector3(240, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Beto)
            {
                var dataPos = new Vector3(0, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Myan)
            {
                var dataPos = new Vector3(10, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
        }
        else if (image_number == 1)
        {
            if (Language == Selects.Japan)
            {
                var dataPos = new Vector3(40, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Nihon)
            {
                var dataPos = new Vector3(40, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.English)
            {
                var dataPos = new Vector3(60, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.China)
            {
                var dataPos = new Vector3(150, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Beto)
            {
                var dataPos = new Vector3(180, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Myan)
            {
                var dataPos = new Vector3(70, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
        }
        else if(image_number == 2)
        {
            if (Language == Selects.Japan)
            {
                var dataPos = new Vector3(0, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Nihon)
            {
                var dataPos = new Vector3(0, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.English)
            {
                var dataPos = new Vector3(0, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.China)
            {
                var dataPos = new Vector3(0, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Beto)
            {
                var dataPos = new Vector3(0, -10, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Myan)
            {
                var dataPos = new Vector3(0, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
        }
        else if (image_number == 3)
        {
            float w = DATA_WIDTH * 650 / 800;
            float h = DATA_WIDTH * textimage_sprite.rect.height / textimage_sprite.rect.width * 650 / 800;
            data_image.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
            if (Language == Selects.English)
            {
                var dataPos = new Vector3(0, -5, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Beto)
            {
                var dataPos = new Vector3(0, -5, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Myan)
            {
                var dataPos = new Vector3(30, 15, 0);
                data_image.transform.localPosition = dataPos;
            }
        }
        else if (image_number == 4)
        {
            float w = DATA_WIDTH * 600 / 800;
            float h = DATA_WIDTH * textimage_sprite.rect.height / textimage_sprite.rect.width * 600 / 800;
            data_image.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
            var dataPos = new Vector3(0, 0, 0);
            data_image.transform.localPosition = dataPos;
        }
        else if(image_number == 5)
        {
            var dataPos = new Vector3(0, 0, 0);
            data_image.transform.localPosition = dataPos;
        }
        else if (image_number == 6)
        {
            float w = DATA_WIDTH * 650 / 800;
            float h = DATA_WIDTH * textimage_sprite.rect.height / textimage_sprite.rect.width * 650 / 800;
            data_image.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
            if (Language == Selects.English)
            {
                var dataPos = new Vector3(0, -5, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Myan)
            {
                var dataPos = new Vector3(0, 10, 0);
                data_image.transform.localPosition = dataPos;
            }
        }
        else if(image_number == 7)
        {
            if (Language == Selects.Japan)
            {
                var dataPos = new Vector3(0, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Nihon)
            {
                var dataPos = new Vector3(120, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.English)
            {
                var dataPos = new Vector3(30, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.China)
            {
                var dataPos = new Vector3(40, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Beto)
            {
                var dataPos = new Vector3(30, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
            else if (Language == Selects.Myan)
            {
                var dataPos = new Vector3(40, 0, 0);
                data_image.transform.localPosition = dataPos;
            }
        }
    }
    private float Get_AspectRate(float width, float height)
    {
        return width / height;
    }
    //問題1のcsvファイルの読み込み関数
    private Item[] ReadFile()
    {
        TextAsset csv = Resources.Load("nagasaki") as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1)
        {
            // ','ごとに区切って配列へ格納
            var line = reader.ReadLine();
            string[] values = line.Split(",");
            this_item = new Item(int.Parse(values[0]), values[1]);
            list_item.Add(this_item);
        }
        Debug.Log(list_item.Count);
        reader.Close();
        return list_item.ToArray();

    }
    public Item[] Get_FileItem()
    {
        return ReadFile();
    }

    //問題2の問題文のcsvファイル読み込み関数
    private Item_Question2_Text[] ReadFile_Question2_Text()
    {
        TextAsset csv = Resources.Load("question2") as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1)
        {
            // ','ごとに区切って配列へ格納
            var line_question2_text = reader.ReadLine();
            string[] values_question2_text = line_question2_text.Split(",");
            this_question2_text = new Item_Question2_Text((values_question2_text[2]), values_question2_text[1]);
            list_question2_text.Add(this_question2_text);

        }
        reader.Close();
        return list_question2_text.ToArray();

    }

    public Item_Question2_Text[] Get_FileItem_Qustion2_Text()
    {
        return ReadFile_Question2_Text();
    }

    //問題2の解答文csvファイルの読み込み関数
    private Item_Question2_Answer[] ReadFile_Question2_Answer() 
    {
        TextAsset csv = Resources.Load("answer-2") as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1)
        {
            // ','ごとに区切って配列へ格納
            var line_question2_answer = reader.ReadLine();
            string[] values_question2_answer = line_question2_answer.Split(",");
            this_question2_answer = new Item_Question2_Answer(int.Parse(values_question2_answer[2]), values_question2_answer[1]);
            list_question2_answer.Add(this_question2_answer);

        }
        reader.Close();
        return list_question2_answer.ToArray();

    }
    public Item_Question2_Answer[] Get_FileItem_Question2_Answer()
    {
        return ReadFile_Question2_Answer();
    }

    //問題3の問題文csvファイルの読み込み関数
    private Item_Question3_Text[] ReadFile_Question3_Text()
    {
        TextAsset csv = Resources.Load("question3-copy") as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1)
        {
            // ','ごとに区切って配列へ格納
            var line_question3_text = reader.ReadLine().Replace("\\n", "\n");
            string[] values_question3_text = line_question3_text.Split(",");
            string[] answer_question3 = line_question3_text.Split(",");
            this_question3_text = new Item_Question3_Text(int.Parse(values_question3_text[0]), values_question3_text[1], answer_question3[2]);
            list_question3_text.Add(this_question3_text);

        }
        reader.Close();
        return list_question3_text.ToArray();
    }
    public Item_Question3_Text[] Get_FileItem_Question3_Text()
    {
        return ReadFile_Question3_Text();
    }

    //Start関数
    void Start()
    {

        //sw = new StreamWriter(@"SaveData.csv", true, Encoding.GetEncoding("Shift_JIS"));

        //各オブジェクトの読み込み
        sign = GameObject.Find("InputField").GetComponent<TMP_InputField>();
        reason = GameObject.Find("InputField_2").GetComponent<TMP_InputField>();
        month = GameObject.Find("InputField_month").GetComponent<TMP_InputField>();
        date = GameObject.Find("InputField_date").GetComponent<TMP_InputField>();
        id = GameObject.Find("InputID").GetComponent<TMP_InputField>();
        language = GameObject.Find("InputLanguage").GetComponent<TMP_InputField>();


        first_images = Resources.LoadAll<Sprite>("nagasaki-image_ver2");
        question_image = GameObject.Find("question-image").GetComponent<Image>();
        QUESTION_WIDTH = question_image.GetComponent<RectTransform>().rect.width;
        QUESTION_HEIGHT = question_image.GetComponent<RectTransform>().rect.height;
        
        consent_image = GameObject.Find("Consent").GetComponent<Image>();
        CONSENT_WIDTH = consent_image.GetComponent<RectTransform>().rect.width;
        CONSENT_HEIGHT = consent_image.GetComponent<RectTransform>().rect.height;

        //data_images = Resources.LoadAll<Sprite>("nagasaki-DataPaper");
        data_image = GameObject.Find("DataImage").GetComponent<Image>();
        DATA_WIDTH = data_image.GetComponent<RectTransform>().rect.width;
        DATA_HEIGHT = data_image.GetComponent<RectTransform>().rect.height;

        title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle");
        title_image = GameObject.Find("QuestionTitle").GetComponent<Image>();
        TITLE_WIDTH = title_image.GetComponent<RectTransform>().rect.width;
        TITLE_HEIGHT = title_image.GetComponent<RectTransform>().rect.height;

        questionText_1_1 = GameObject.Find("Text_1_1").GetComponent<Image>();
        questionText_1_2 = GameObject.Find("Text_1_2").GetComponent<Image>();
        questionText_1_3 = GameObject.Find("Text_1_3").GetComponent<Image>();
        questionText_1_4 = GameObject.Find("Text_1_4").GetComponent<Image>();
        questionText_other = GameObject.Find("Text_1_5").GetComponent<Image>();



        answer1 = GameObject.Find("answer1").GetComponent<TextMeshProUGUI>();
        answer2 = GameObject.Find("answer2").GetComponent<TextMeshProUGUI>();
        answer3 = GameObject.Find("answer3").GetComponent<TextMeshProUGUI>();
        answer4 = GameObject.Find("answer4").GetComponent<TextMeshProUGUI>();

        question2_japan = GameObject.Find("question2_japan").GetComponent<TextMeshProUGUI>();
        question2_english = GameObject.Find("question2_english").GetComponent<TextMeshProUGUI>();
        question3_text = GameObject.Find("Question3_Text").GetComponent<TextMeshProUGUI>();

        question3 = GameObject.Find("Question3").GetComponent<RubyTextMeshProUGUI>();

        questionImage = GameObject.Find("questionImage").GetComponent<Image>();
        englishImage = GameObject.Find("englishImage").GetComponent<Image>();
        questionText_2_1 = GameObject.Find("Text_2_1").GetComponent<Image>();
        questionText_2_2 = GameObject.Find("Text_2_2").GetComponent<Image>();
        questionText_2_3 = GameObject.Find("Text_2_3").GetComponent<Image>();
        questionText_2_4 = GameObject.Find("Text_2_4").GetComponent<Image>();

        questionImage3 = GameObject.Find("question3Image").GetComponent<Image>();
        QUESTION3_HEIGHT = questionImage3.GetComponent<RectTransform>().rect.height;

        yesImage = GameObject.Find("yesText").GetComponent<Image>();
        noImage = GameObject.Find("noText").GetComponent<Image>();
        etcImage = GameObject.Find("etcText").GetComponent<Image>();

        answer5 = GameObject.Find("answer5").GetComponent<TextMeshProUGUI>();
        answer6 = GameObject.Find("answer6").GetComponent<TextMeshProUGUI>();
        answer7 = GameObject.Find("answer7").GetComponent<TextMeshProUGUI>();
        answer8 = GameObject.Find("answer8").GetComponent<TextMeshProUGUI>();

        title = GameObject.Find("Title").GetComponent<Canvas>();
        canvas1 = GameObject.Find("Canvas1").GetComponent<Canvas>();
        canvas2 = GameObject.Find("Canvas2").GetComponent<Canvas>();
        canvas3 = GameObject.Find("Canvas3").GetComponent<Canvas>();
        consent = GameObject.Find("ConsentImage").GetComponent<Canvas>();
        data_canvas = GameObject.Find("DataPaper").GetComponent<Canvas>();

        Consent_Next = GameObject.Find("Next_Con").GetComponent<Button>();
        Data_Next = GameObject.Find("Next_Data").GetComponent<Button>();
        Question1_Next = GameObject.Find("Next").GetComponent<Button>();
        Question2_Next = GameObject.Find("Next_2").GetComponent<Button>();
        Question3_Next = GameObject.Find("Next_3").GetComponent<Button>();
        //circle = GameObject.Find("CircleButton").GetComponent<Button>();
        //cross = GameObject.Find("CrossButton").GetComponent<Button>();

        Consent_Back = GameObject.Find("Back_Con").GetComponent<Button>();
        Data_back = GameObject.Find("Back_Data").GetComponent<Button>();
        Question1_Back = GameObject.Find("Back").GetComponent<Button>();
        Question2_Back = GameObject.Find("Back_2").GetComponent<Button>();
        Question3_Back = GameObject.Find("Back_3").GetComponent<Button>();
        Consent_Change_Button = GameObject.Find("Consent_Change").GetComponent<Button>();
        Data_Change_Button = GameObject.Find("Data_Change").GetComponent<Button>();
        Question1_Change = GameObject.Find("QuestionChange").GetComponent<Button>();
        Question2_Change = GameObject.Find("QuestionChange_2").GetComponent<Button>();
        End_Button = GameObject.Find("End_Button").GetComponent<Button>();

        backTitle_Button = GameObject.Find("Title_Button").GetComponent<Button>();
        backTitle_Button2 = GameObject.Find("backButton").GetComponent<Button>();

        Title_Change_Button = GameObject.Find("Title_Change_Button").GetComponent<Button>();
        //Reset_Button = GameObject.Find("Button").GetComponent<Button>();

        con_toggle1 = GameObject.Find("Con_Toggle1").GetComponent<Toggle>();
        con_toggle2 = GameObject.Find("Con_Toggle2").GetComponent<Toggle>();
        con_toggle3 = GameObject.Find("Con_Toggle3").GetComponent<Toggle>();
        con_toggle4 = GameObject.Find("Con_Toggle4").GetComponent<Toggle>();
        con_toggle5 = GameObject.Find("Con_Toggle5").GetComponent<Toggle>();
        con_toggle6 = GameObject.Find("Con_Toggle6").GetComponent<Toggle>();
        understandToggle = GameObject.Find("Con_Toggle_nihon").GetComponent<Toggle>();

        japan_agree = GameObject.Find("Japan_agree").GetComponent<Toggle>();
        japan_disagree = GameObject.Find("Japan_disagree").GetComponent<Toggle>();
        japan_etc = GameObject.Find("Japan_etc").GetComponent<Toggle>();

        ageYes = GameObject.Find("ageYes").GetComponent<Toggle>();
        ageNo = GameObject.Find("ageNo").GetComponent<Toggle>();
        consentYes = GameObject.Find("consentYes").GetComponent<Toggle>();
        consentNo = GameObject.Find("consentNo").GetComponent<Toggle>();

        data_toggle1 = GameObject.Find("Data_Toggle1").GetComponent<Toggle>();
        data_toggle2 = GameObject.Find("Data_Toggle2").GetComponent<Toggle>();
        data_toggle3 = GameObject.Find("Data_Toggle3").GetComponent<Toggle>();
        data_toggle4 = GameObject.Find("Data_Toggle4").GetComponent<Toggle>();
        data_toggle5 = GameObject.Find("Data_Toggle5").GetComponent<Toggle>();
        data_toggle6 = GameObject.Find("Data_Toggle6").GetComponent<Toggle>();
        data_toggle7 = GameObject.Find("Data_Toggle7").GetComponent<Toggle>();
        data_toggle8 = GameObject.Find("Data_Toggle8").GetComponent<Toggle>();
        data_toggle9 = GameObject.Find("Data_Toggle9").GetComponent<Toggle>();
        data_toggle10 = GameObject.Find("Data_Toggle10").GetComponent<Toggle>();
        data_toggle11 = GameObject.Find("Data_Toggle11").GetComponent<Toggle>();
        data_toggle12 = GameObject.Find("Data_Toggle12").GetComponent<Toggle>();
        data_toggle13 = GameObject.Find("Data_Toggle13").GetComponent<Toggle>();
        data_toggle14 = GameObject.Find("Data_Toggle14").GetComponent<Toggle>();
        data_toggle15 = GameObject.Find("Data_Toggle15").GetComponent<Toggle>();
        data_toggle16 = GameObject.Find("Data_Toggle16").GetComponent<Toggle>();

        data_toggle17 = GameObject.Find("Data_Toggle16 (1)").GetComponent<Toggle>();
        data_toggle18 = GameObject.Find("Data_Toggle16 (2)").GetComponent<Toggle>();
        data_toggle19 = GameObject.Find("Data_Toggle16 (3)").GetComponent<Toggle>();
        data_toggle20 = GameObject.Find("Data_Toggle16 (4)").GetComponent<Toggle>();
        data_toggle21 = GameObject.Find("Data_Toggle16 (5)").GetComponent<Toggle>();

        data_toggle_language1 = GameObject.Find("Data_Toggle_Language").GetComponent<Toggle>();
        data_toggle_language2 = GameObject.Find("Data_Toggle_Language (1)").GetComponent<Toggle>();
        data_toggle_language3 = GameObject.Find("Data_Toggle_Language (2)").GetComponent<Toggle>();
        data_toggle_language4 = GameObject.Find("Data_Toggle_Language (3)").GetComponent<Toggle>();
        data_toggle_language5 = GameObject.Find("Data_Toggle_Language (4)").GetComponent<Toggle>();
        data_toggle_language6 = GameObject.Find("Data_Toggle_Language (5)").GetComponent<Toggle>();
        data_toggle_language7 = GameObject.Find("Data_Toggle_Language (6)").GetComponent<Toggle>();
        data_toggle_language8 = GameObject.Find("Data_Toggle_Language (7)").GetComponent<Toggle>();
        data_toggle_language9 = GameObject.Find("Data_Toggle_Language (8)").GetComponent<Toggle>();
        data_toggle_language10 = GameObject.Find("Data_Toggle_Language (9)").GetComponent<Toggle>();
        data_toggle_language11 = GameObject.Find("Data_Toggle_Language (10)").GetComponent<Toggle>();
        data_toggle_language12 = GameObject.Find("Data_Toggle_Language (11)").GetComponent<Toggle>();
        data_toggle_language13 = GameObject.Find("Data_Toggle_Language (12)").GetComponent<Toggle>();
        data_toggle_language14 = GameObject.Find("Data_Toggle_Language (13)").GetComponent<Toggle>();
        data_toggle_language15 = GameObject.Find("Data_Toggle_Language (14)").GetComponent<Toggle>();
        data_toggle_language16 = GameObject.Find("Data_Toggle_Language (15)").GetComponent<Toggle>();

        toggle1 = GameObject.Find("Toggle").GetComponent<Toggle>();
        toggle2 = GameObject.Find("Toggle (1)").GetComponent<Toggle>();
        toggle3 = GameObject.Find("Toggle (2)").GetComponent<Toggle>();
        toggle4 = GameObject.Find("Toggle (3)").GetComponent<Toggle>();
        toggle_other = GameObject.Find("Toggle_Other").GetComponent<Toggle>();
        toggle5 = GameObject.Find("Toggle (4)").GetComponent<Toggle>();
        toggle6 = GameObject.Find("Toggle (5)").GetComponent<Toggle>();
        toggle7 = GameObject.Find("Toggle (6)").GetComponent<Toggle>();
        toggle8 = GameObject.Find("Toggle (7)").GetComponent<Toggle>();

        yes = GameObject.Find("yes").GetComponent<Toggle>();
        no = GameObject.Find("no").GetComponent<Toggle>();
        etc = GameObject.Find("etc").GetComponent<Toggle>();
        //Random();
        ReadFile_Item = ReadFile();
        ReadFile_Item_Question2_Text = ReadFile_Question2_Text();
        ReadFile_Item_Question2_Answer = ReadFile_Question2_Answer();
        ReadFile_Item_Question3_Text = ReadFile_Question3_Text();

        int[] NUMBER_1 = new int[NUMBER_OF_QUESTION_1 * FONT_COUNT]; //0～20までを3セット入れた配列
        int[] NUMBER_2 = new int[NUMBER_OF_QUESTION_2 * FONT_COUNT]; //0～7までを3セット入れた配列
        int[] NUMBER_3 = new int[NUMBER_OF_QUESTION_3 * FONT_COUNT]; //0～20までを3セット入れた配列
        int[] FontNumber = new int[maxQuestionpattern * FONT_COUNT]; //0～3までを21セットいれた配列

        int[] Random_number = new int[QUESTION_PATTERN_1];
        int[] Random_number_2 = new int[QUESTION_PATTERN_2];
        int[] Random_number_3 = new int[QUESTION_PATTERN_3];

        int[] Answer_num_1 = new int[QUESTION_PATTERN_1 * NUMBER_OF_ANSWER * FONT_COUNT];
        int[] Answer_num_2 = new int[QUESTION_PATTERN_2 * NUMBER_OF_ANSWER * FONT_COUNT];
        ////////////////////////////////////////////////////////////////////////////////////////////
        // 問題の配列生成

        //問題番号、解答の参照先ををシャッフル生成する
        for (int i = 0; i < QUESTION_PATTERN_1; i++)
        {
            Random_number[i] = i; 
        }
        for(int i = 0; i < QUESTION_PATTERN_2; i++)
        {
            Random_number_2[i] = i;
        }
        for(int i = 0; i < QUESTION_PATTERN_3; i++)
        {
            Random_number_3[i] = i;
        }
        Random_number.Shuffle();
        Random_number_2.Shuffle();
        Random_number_3.Shuffle();

        for(int i=0; i< QUESTION_PATTERN_1 * NUMBER_OF_ANSWER * FONT_COUNT; i = i + NUMBER_OF_ANSWER)
        {
            for(int j =0; j < NUMBER_OF_ANSWER; j++)
            {
                Answer_num_1[i + j] = j;
            }
        }
        for (int i = 0; i < QUESTION_PATTERN_2 * NUMBER_OF_ANSWER * FONT_COUNT; i = i + (QUESTION_PATTERN_2 * NUMBER_OF_ANSWER))
        {
            for (int j = 0; j < NUMBER_OF_ANSWER; j++)
            {
                Answer_num_2[i + j] = j;
            }
        }
        Answer_num_1.Answer_Shuffle();
        Answer_num_2.Answer_Shuffle();


        //問題番号を3セットずつ生成する
        for (int i = 0; i < (NUMBER_OF_QUESTION_1 * FONT_COUNT); i = i + NUMBER_OF_QUESTION_1) //0～20までを一定セット入れた配列を生成
        {
            for (int j = 0; j < NUMBER_OF_QUESTION_1; j++)
            {
                NUMBER_1[i + j] = Random_number[j];
            }
        }
        for(int i=0; i < (NUMBER_OF_QUESTION_2 * FONT_COUNT); i = i + NUMBER_OF_QUESTION_2)
        {
            for(int j = 0; j < NUMBER_OF_QUESTION_2; j++)
            {
                NUMBER_2[i + j] = Random_number_2[j];
            }
        }
        for(int i=0; i < (NUMBER_OF_QUESTION_3 * FONT_COUNT); i= i + NUMBER_OF_QUESTION_3)
        {
            for(int j = 0; j < NUMBER_OF_QUESTION_3; j++)
            {
                NUMBER_3[i + j] = Random_number_3[j];
            }
        }
        NUMBER_1.Number_Shuffle();    //0～21の配列のシャッフル
        NUMBER_2.Number_Shuffle();    //0～7の配列のシャッフル
        NUMBER_3.Number_Shuffle();    //0～21の配列のシャッフル
        //Question_number配列に入れる
        int k = 0;
        for (int i = 0; i < FONT_COUNT; i++)       //Question_number配列に入れる
        {
            for (int j = 0; j < NUMBER_OF_QUESTION_1; j++)
            {
                Question_number_1[i, j] = NUMBER_1[k];
                k++;
            }
        }
        k = 0;
        for (int i = 0; i < FONT_COUNT; i++)       //Question_number配列に入れる
        {
            for (int j = 0; j < NUMBER_OF_QUESTION_2; j++)
            {
                Question_number_2[i, j] = NUMBER_2[k];
                k++;
            }
        }
        k = 0;
        for (int i = 0; i < FONT_COUNT; i++)       //Question_number配列に入れる
        {
            for (int j = 0; j < NUMBER_OF_QUESTION_3; j++)
            {
                Question_number_3[i, j] = NUMBER_3[k];
                k++;
            }
        }

        k = 0;
        for (int i = 0; i < FONT_COUNT; i++)
        {
            for (int j = 0; j < QUESTION_PATTERN_1; j++)
            {
                for (int z = 0; z < NUMBER_OF_ANSWER; z++)
                {
                    csv_answers_num1[i, j, z] = Answer_num_1[k];
                    k++;
                }
            }
        }
        k = 0;
        for (int i = 0; i < FONT_COUNT; i++)
        {
            for (int j = 0; j < QUESTION_PATTERN_2; j++)
            {
                for (int z = 0; z < NUMBER_OF_ANSWER; z++)
                {
                    csv_answers_num2[i, j, z] = Answer_num_1[k];
                    k++;
                }
            }
        }

        string[] csv_answers_1 = new string[QUESTION_PATTERN_1 * NUMBER_OF_ANSWER];
        string[] csv_answers_2 = new string[QUESTION_PATTERN_2 * NUMBER_OF_ANSWER];
        string[] csv_answers_3 = new string[QUESTION_PATTERN_3 * NUMBER_OF_ANSWER];
        string[] answers_array_1 = new string[QUESTION_PATTERN_1 * NUMBER_OF_ANSWER * FONT_COUNT];
        string[] answers_array_2 = new string[QUESTION_PATTERN_2 * NUMBER_OF_ANSWER * FONT_COUNT];
        string[] answers_array_3 = new string[QUESTION_PATTERN_3 * NUMBER_OF_ANSWER * FONT_COUNT];
        //csvファイルの内容をそれぞれ取り込む
        k = 0;
        foreach (Item item in ReadFile_Item)
        {
            csv_answers_1[k] = item.Value;
            k++;
        }
        k = 0;
        foreach (Item_Question2_Text question2_text in ReadFile_Item_Question2_Text)
        {
            question_Text[k] = new Question_Text(question2_text.Number_Question2_Text, question2_text.Value_Question2_Text);
            k++;
        }
        k = 0;
        foreach (Item_Question2_Answer item in ReadFile_Item_Question2_Answer)
        {
            csv_answers_2[k] = item.Value_Question2_Answer;
            k++;
        }
        k = 0;
        foreach (Item_Question3_Text item in ReadFile_Item_Question3_Text)
        {
            Question3_Text[k] = item.Value_Question3_Text;
            question3Answer[k] = item.Answer_question3;
            k++;
        }

        //それぞれフォントの数だけ生成する
        for (int i = 0; i < QUESTION_PATTERN_1 * NUMBER_OF_ANSWER * FONT_COUNT; i = i + (QUESTION_PATTERN_1 * NUMBER_OF_ANSWER))
        {
            for (int j = 0; j < QUESTION_PATTERN_1 * NUMBER_OF_ANSWER; j++)
            {
                answers_array_1[i + j] = csv_answers_1[j];
            }
        }
        for (int i = 0; i < QUESTION_PATTERN_2 * NUMBER_OF_ANSWER * FONT_COUNT; i = i + (QUESTION_PATTERN_2 * NUMBER_OF_ANSWER))
        {
            for (int j = 0; j < QUESTION_PATTERN_2 * NUMBER_OF_ANSWER; j++)
            {
                answers_array_2[i + j] = csv_answers_2[j];
            }
        }

        //最終的な配列にいれる
        k = 0;
        for (int i = 0; i < QUESTION_PATTERN_1; i++)
        {
            for (int j = 0; j < NUMBER_OF_ANSWER; j++)
            {
                Csv_answers_1[i, j] = answers_array_1[k];
                k++;
            }
        }
        k = 0;
        for (int i = 0; i < QUESTION_PATTERN_2; i++)
        {
            for (int j = 0; j < NUMBER_OF_ANSWER; j++)
            {
                Csv_answers_2[i, j] = answers_array_2[k];
                k++;
            }
        }

        


        for (int i = 0; i < maxQuestionpattern * FONT_COUNT ; i = i + FONT_COUNT) //0～3までを一定セット入れた配列を生成
        {
            for(int j = 0; j < FONT_COUNT; j++)
            {
                FontNumber[i + j] = j;
            }
        }
        FontNumber.Font_Shuffle();  //0～3の配列のシャッフルs

        k = 0;
        for (int i = 0; i < maxQuestionpattern; i++) // FontArray配列に入れる
        {
            for (int j = 0; j < FONT_COUNT; j++)
            {
                FontArray[i, j] = FontNumber[k];
                k++;
            }
        }
        X = Data_Next.transform.localPosition.x;
        X2 = Consent_Next.transform.localPosition.x;
        Y = Data_Next.transform.localPosition.y;
        Y2 = Consent_Next.transform.localPosition.y;
        Z = 0;
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        NotActiveObjects();
        material1.gameObject.SetActive(false);
        material2.gameObject.SetActive(false);
        material3.gameObject.SetActive(false);
        material4.gameObject.SetActive(false);
        material_other.gameObject.SetActive(false);
        material5.gameObject.SetActive(false);
        material6.gameObject.SetActive(false);
        material7.gameObject.SetActive(false);
        material8.gameObject.SetActive(false);
        material9.gameObject.SetActive(false);
        material10.gameObject.SetActive(false);
        material11.gameObject.SetActive(false);
        CanvasManager();
    }

    //更新関数
    private void Update()
    {
        //Consent
        if (this.mode == Mode.Consent_Main)
        {
            if (image_number == 0)
            {
                Consent_Back.gameObject.SetActive(false);
                Consent_Next.gameObject.SetActive(true);
                Consent_Change_Button.gameObject.SetActive(false);
            }
            else
            {
                Consent_Back.gameObject.SetActive(true);
            }
            if(image_number == 1)
            {
                if (con_toggle1.isOn == true || con_toggle2.isOn == true || con_toggle3.isOn == true || con_toggle4.isOn == true || con_toggle5.isOn == true || con_toggle6.isOn == true)
                {
                    Consent_Next.gameObject.SetActive(true);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
                else
                {
                    Consent_Back.gameObject.SetActive(true);
                    Consent_Next.gameObject.SetActive(false);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
            }
            if(image_number == 2 || image_number == 3)
            {
                Consent_Back.gameObject.SetActive(true);
                Consent_Next.gameObject.SetActive(true);
                Consent_Change_Button.gameObject.SetActive(false);
            }
            if(image_number == 4)
            {
                if(understandToggle.isOn == true)
                {
                    Consent_Next.gameObject.SetActive(true);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
                else
                {
                    Consent_Next.gameObject.SetActive(false);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
            }
            if(image_number == 5)
            {
                if (japan_agree.isOn == true || japan_disagree.isOn == true || japan_etc.isOn == true)
                {
                    Consent_Next.gameObject.SetActive(true);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
                else
                {
                    Consent_Back.gameObject.SetActive(true);
                    Consent_Next.gameObject.SetActive(false);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
            }
            if(image_number == 6) //同意しなかった画面
            {
                Consent_Back.gameObject.SetActive(true);
                Consent_Next.gameObject.SetActive(false);
                Consent_Change_Button.gameObject.SetActive(false);

                // ここにタイトルに戻るボタンを入れるのはあり
                // 要検討
            }

            if(image_number == 7)
            {
                if (reason.text != "")
                {
                    Consent_Next.gameObject.SetActive(true);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
                else
                {
                    Consent_Next.gameObject.SetActive(false);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
            }
            if(image_number　== 8)
            {
                if(ageYes.isOn == true || ageNo.isOn == true)
                {
                    Consent_Next.gameObject.SetActive(true);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
                else
                {
                    Consent_Next.gameObject.SetActive(false);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
            }
            if(image_number == 9)
            {
                if (consentYes.isOn == true)
                {
                    Consent_Next.gameObject.SetActive(true);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
                else
                {
                    Consent_Next.gameObject.SetActive(false);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
            }
            if(image_number == 10)
            {
                if (month.text != "" && date.text != "" && sign.text != "")
                {
                    Consent_Next.gameObject.SetActive(true);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
                else
                {
                    Consent_Next.gameObject.SetActive(false);
                    Consent_Change_Button.gameObject.SetActive(false);
                }
            }
            if (image_number == 11)
            {
                Consent_Back.gameObject.SetActive(true);
                Consent_Next.gameObject.SetActive(false);
                Consent_Change_Button.gameObject.SetActive(true);
            }

            ////////////////////////////////////////////////////////////
            //同意画面のトグル処理
            if (con_toggle1.isOn == true)
            {
                con_material1.gameObject.SetActive(true);
                con_material1.color = new Color32(50, 50, 50, 100);
            }
            if (con_toggle2.isOn == true)
            {
                con_material2.gameObject.SetActive(true);
                con_material2.color = new Color32(50, 50, 50, 100);
            }
            if (con_toggle3.isOn == true)
            {
                con_material3.gameObject.SetActive(true);
                con_material3.color = new Color32(50, 50, 50, 100);
            }
            if (con_toggle4.isOn == true)
            {
                con_material4.gameObject.SetActive(true);
                con_material4.color = new Color32(50, 50, 50, 100);
            }
            if (con_toggle5.isOn == true)
            {
                con_material5.gameObject.SetActive(true);
                con_material5.color = new Color32(50, 50, 50, 100);
            }
            if (con_toggle6.isOn == true)
            {
                con_material6.gameObject.SetActive(true);
                con_material6.color = new Color32(50, 50, 50, 100);
            }
            //////////////////////////////////////////////////////////////////////
            //各言語トグルの処理
            if (understandToggle.isOn == true)
            {
                understandMate.gameObject.SetActive(true);
                understandMate.color = new Color32(50, 50, 50, 100);
            }

            if (japan_agree.isOn == true)
            {
                con_mateJapan_agree.gameObject.SetActive(true);
                con_mateJapan_agree.color = new Color32(50, 50, 50, 100);
            }
            if (japan_disagree.isOn == true)
            {
                con_mateJapan_disagree.gameObject.SetActive(true);
                con_mateJapan_disagree.color = new Color32(50, 50, 50, 100);
            }
            if (japan_etc.isOn == true)
            {
                con_mateJapan_etc.gameObject.SetActive(true);
                con_mateJapan_etc.color = new Color32(50, 50, 50, 100);
            }

            if (ageYes.isOn == true)
            {
                ageYes_material.gameObject.SetActive(true);
                ageYes_material.color = new Color32(50, 50, 50, 100);
            }
            if (ageNo.isOn == true)
            {
                ageNo_material.gameObject.SetActive(true);
                ageNo_material.color = new Color32(50, 50, 50, 100);
            }
            if (consentYes.isOn == true)
            {
                consentYes_material.gameObject.SetActive(true);
                consentYes_material.color = new Color32(50, 50, 50, 100);
            }
            ///////////////////////////////////////////////////////////////////////////
        }
        //Data
        if (this.mode == Mode.Data)
        {
            //data
            if (data_toggle1.isOn == true)
            {
                data_material1.gameObject.SetActive(true);
                data_material1.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 0] = 1;
            }
            if (data_toggle2.isOn == true)
            {
                data_material2.gameObject.SetActive(true);
                data_material2.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 1] = 1;
            }
            if (data_toggle3.isOn == true)
            {
                data_material3.gameObject.SetActive(true);
                data_material3.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 2] = 1;
            }
            if (data_toggle4.isOn == true)
            {
                data_material4.gameObject.SetActive(true);
                data_material4.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 3] = 1;
            }
            if (data_toggle5.isOn == true)
            {
                data_material5.gameObject.SetActive(true);
                data_material5.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 4] = 1;
            }
            ///////////////////////////////////////////////////////////////////////////////
            if (data_toggle6.isOn == true)
            {
                data_material6.gameObject.SetActive(true);
                data_material6.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 0] = 1;
            }
            if (data_toggle7.isOn == true)
            {
                data_material7.gameObject.SetActive(true);
                data_material7.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 1] = 1;
            }
            if (data_toggle8.isOn == true)
            {
                data_material8.gameObject.SetActive(true);
                data_material8.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 2] = 1;
            }
            if (data_toggle9.isOn == true)
            {
                data_material9.gameObject.SetActive(true);
                data_material9.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 3] = 1;
            }
            if (data_toggle10.isOn == true)
            {
                data_material10.gameObject.SetActive(true);
                data_material10.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 4] = 1;
            }
            /////////////////////////////////////////////////////////////////////////////
            if (data_toggle11.isOn == true)
            {
                data_material11.gameObject.SetActive(true);
                data_material11.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 0] = 1;
            }
            if (data_toggle12.isOn == true)
            {
                data_material12.gameObject.SetActive(true);
                data_material12.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 1] = 1;
            }
            if (data_toggle13.isOn == true)
            {
                data_material13.gameObject.SetActive(true);
                data_material13.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 2] = 1;
            }
            if (data_toggle14.isOn == true)
            {
                data_material14.gameObject.SetActive(true);
                data_material14.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 3] = 1;
            }
            if (data_toggle15.isOn == true)
            {
                data_material15.gameObject.SetActive(true);
                data_material15.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 4] = 1;
            }
            if (data_toggle16.isOn == true)
            {
                data_material16.gameObject.SetActive(true);
                data_material16.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 5] = 1;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////
            ///
            if (data_toggle17.isOn == true)
            {
                data_material17.gameObject.SetActive(true);
                data_material17.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 5] = 1;
            }
            if (data_toggle18.isOn == true)
            {
                data_material18.gameObject.SetActive(true);
                data_material18.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 5] = 1;
            }
            if (data_toggle19.isOn == true)
            {
                data_material19.gameObject.SetActive(true);
                data_material19.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 5] = 1;
            }
            if (data_toggle20.isOn == true)
            {
                data_material20.gameObject.SetActive(true);
                data_material20.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 5] = 1;
            }
            if (data_toggle21.isOn == true)
            {
                data_material21.gameObject.SetActive(true);
                data_material21.color = new Color32(50, 50, 50, 100);
                //Toggle_selects[image_number, 5] = 1;
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            if (data_toggle_language1.isOn == true)
            {
                data_material_Language1.gameObject.SetActive(true);
                data_material_Language1.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language2.isOn == true)
            {
                data_material_Language2.gameObject.SetActive(true);
                data_material_Language2.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language3.isOn == true)
            {
                data_material_Language3.gameObject.SetActive(true);
                data_material_Language3.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language4.isOn == true)
            {
                data_material_Language4.gameObject.SetActive(true);
                data_material_Language4.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language5.isOn == true)
            {
                data_material_Language5.gameObject.SetActive(true);
                data_material_Language5.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language6.isOn == true)
            {
                data_material_Language6.gameObject.SetActive(true);
                data_material_Language6.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language7.isOn == true)
            {
                data_material_Language7.gameObject.SetActive(true);
                data_material_Language7.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language8.isOn == true)
            {
                data_material_Language8.gameObject.SetActive(true);
                data_material_Language8.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language9.isOn == true)
            {
                data_material_Language9.gameObject.SetActive(true);
                data_material_Language9.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language10.isOn == true)
            {
                data_material_Language10.gameObject.SetActive(true);
                data_material_Language10.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language11.isOn == true)
            {
                data_material_Language11.gameObject.SetActive(true);
                data_material_Language11.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language12.isOn == true)
            {
                data_material_Language12.gameObject.SetActive(true);
                data_material_Language12.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language13.isOn == true)
            {
                data_material_Language13.gameObject.SetActive(true);
                data_material_Language13.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language14.isOn == true)
            {
                data_material_Language14.gameObject.SetActive(true);
                data_material_Language14.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language15.isOn == true)
            {
                data_material_Language15.gameObject.SetActive(true);
                data_material_Language15.color = new Color32(50, 50, 50, 100);
            }
            if (data_toggle_language16.isOn == true)
            {
                data_material_Language16.gameObject.SetActive(true);
                data_material_Language16.color = new Color32(50, 50, 50, 100);
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////
            ///

            if (image_number == 0)
            {
                Data_back.gameObject.SetActive(false);
                Data_Next.gameObject.SetActive(true);
            }
            else if(image_number == 1)
            {
                Data_back.gameObject.SetActive(true);
                Data_Next.gameObject.SetActive(false);
                Data_Change_Button.gameObject.SetActive(false);
                if (id.text != "")
                {
                    Data_Next.gameObject.SetActive(true);
                    Data_Change_Button.gameObject.SetActive(false);
                }
            }
            else if(image_number == 2)
            {
                Data_back.gameObject.SetActive(true);
                Data_Next.gameObject.SetActive(false);
                Data_Change_Button.gameObject.SetActive(false);
                if(data_toggle_language1.isOn == true || data_toggle_language2.isOn == true || data_toggle_language3.isOn == true || data_toggle_language4.isOn == true ||
                    data_toggle_language5.isOn == true || data_toggle_language6.isOn == true || data_toggle_language7.isOn == true || data_toggle_language8.isOn == true ||
                    data_toggle_language9.isOn == true || data_toggle_language10.isOn == true || data_toggle_language11.isOn == true || data_toggle_language12.isOn == true ||
                    data_toggle_language13.isOn == true || data_toggle_language14.isOn == true || data_toggle_language16.isOn == true)
                {
                    language.gameObject.SetActive(false);
                    Data_Next.gameObject.SetActive(true);
                    Data_Change_Button.gameObject.SetActive(false);
                }
                if(data_toggle_language15.isOn == true)
                {
                    language.gameObject.SetActive(true);
                    Data_Next.gameObject.SetActive(false);
                    Data_Change_Button.gameObject.SetActive(false);
                    if (language.text != "")
                    {
                        Data_Next.gameObject.SetActive(true);
                        Data_Change_Button.gameObject.SetActive(false);
                    }
                }
            }
            else if (image_number != NUMBER_OF_DATAIMAGES && image_number > 2)
            {
                if (data_toggle1.isOn == true || data_toggle2.isOn == true || data_toggle3.isOn == true || data_toggle4.isOn == true || data_toggle5.isOn == true || data_toggle6.isOn == true ||
                   data_toggle7.isOn == true || data_toggle8.isOn == true || data_toggle9.isOn == true || data_toggle10.isOn == true)
                {
                    Data_Next.gameObject.SetActive(true);
                    Data_Change_Button.gameObject.SetActive(false);
                }
                else if (data_toggle11.isOn == true || data_toggle12.isOn == true || data_toggle13.isOn == true || data_toggle14.isOn == true || data_toggle15.isOn == true || data_toggle16.isOn == true)
                {
                    Data_Next.gameObject.SetActive(true);
                    Data_Change_Button.gameObject.SetActive(false);
                }
                else if (data_toggle17.isOn == true || data_toggle18.isOn == true || data_toggle19.isOn == true || data_toggle20.isOn == true || data_toggle21.isOn == true)
                {
                    Data_Next.gameObject.SetActive(true);
                    Data_Change_Button.gameObject.SetActive(false);
                }
            }
            else 
            {
                Data_back.gameObject.SetActive(true);
                Data_Next.gameObject.SetActive(false);
                Data_Change_Button.gameObject.SetActive(true);
            }
            
        }

        //問題1
        if (this.mode == Mode.Question1)
        {
            //問題1のトグル管理
            if (toggle1.isOn == true)
            {
                toggle_flag = 1;
                material1.gameObject.SetActive(true);
                material1.color = new Color32(50, 50, 50, 100);
            }
            if (toggle2.isOn == true)
            {
                toggle_flag = 2;
                material2.gameObject.SetActive(true);
                material2.color = new Color32(50, 50, 50, 100);
            }
            if (toggle3.isOn == true)
            {
                toggle_flag = 3;
                material3.gameObject.SetActive(true);
                material3.color = new Color32(50, 50, 50, 100);
            }
            if (toggle4.isOn == true)
            {
                toggle_flag = 4;
                material4.gameObject.SetActive(true);
                material4.color = new Color32(50, 50, 50, 100);
            }
            if (toggle_other.isOn == true)
            {
                toggle_flag = 5;
                material_other.gameObject.SetActive(true);
                material_other.color = new Color32(50, 50, 50, 100);
            }
            ///////////////////////////////////////////////////////
            if (round != FONT_COUNT - 1)
            {
                if (toggle1.isOn == true || toggle2.isOn == true || toggle3.isOn == true || toggle4.isOn == true || toggle_other.isOn == true)
                {
                    Question1_Next.gameObject.SetActive(true);
                    Question1_Change.gameObject.SetActive(false);
                }
                else
                {
                    Question1_Next.gameObject.SetActive(false);
                    Question1_Change.gameObject.SetActive(false);
                }
            }
            else
            {
                if (image_number != NUMBER_OF_QUESTION_1 - 1)
                {
                    if (toggle1.isOn == true || toggle2.isOn == true || toggle3.isOn == true || toggle4.isOn == true || toggle_other.isOn == true)
                    {
                        Question1_Next.gameObject.SetActive(true);
                        Question1_Change.gameObject.SetActive(false);
                    }
                    else
                    {
                        Question1_Next.gameObject.SetActive(false);
                        Question1_Change.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (toggle1.isOn == true || toggle2.isOn == true || toggle3.isOn == true || toggle4.isOn == true || toggle_other.isOn == true)
                    {
                        Question1_Next.gameObject.SetActive(false);
                        Question1_Change.gameObject.SetActive(true);
                    }
                    else
                    {
                        Question1_Next.gameObject.SetActive(false);
                        Question1_Change.gameObject.SetActive(false);
                    }
                }
            }
        }
        //問題2
        if (this.mode == Mode.Question2)
        {
            //問題2のトグル管理
            if (toggle5.isOn == true)
            {
                toggle_flag = 1;
                material5.gameObject.SetActive(true);
                material5.color = new Color32(50, 50, 50, 100);
            }
            if (toggle6.isOn == true)
            {
                toggle_flag = 2;
                material6.gameObject.SetActive(true);
                material6.color = new Color32(50, 50, 50, 100);
            }
            if (toggle7.isOn == true)
            {
                toggle_flag = 3;
                material7.gameObject.SetActive(true);
                material7.color = new Color32(50, 50, 50, 100);
            }
            if (toggle8.isOn == true)
            {
                toggle_flag = 4;
                material8.gameObject.SetActive(true);
                material8.color = new Color32(50, 50, 50, 100);
            }

            if (round != FONT_COUNT - 1)
            {
                if (toggle5.isOn == true || toggle6.isOn == true || toggle7.isOn == true || toggle8.isOn == true)
                {
                    Question2_Next.gameObject.SetActive(true);
                    Question2_Change.gameObject.SetActive(false);
                }
                else
                {
                    Question2_Next.gameObject.SetActive(false);
                    Question2_Change.gameObject.SetActive(false);
                }
            }
            else
            {
                if (image_number != NUMBER_OF_QUESTION_2 - 1)
                {
                    if (toggle5.isOn == true || toggle6.isOn == true || toggle7.isOn == true || toggle8.isOn == true)
                    {
                        Question2_Next.gameObject.SetActive(true);
                        Question2_Change.gameObject.SetActive(false);
                    }
                    else
                    {
                        Question2_Next.gameObject.SetActive(false);
                        Question2_Change.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (toggle5.isOn == true || toggle6.isOn == true || toggle7.isOn == true || toggle8.isOn == true)
                    {
                        Question2_Next.gameObject.SetActive(false);
                        Question2_Change.gameObject.SetActive(true);
                    }
                    else
                    {
                        Question2_Next.gameObject.SetActive(false);
                        Question2_Change.gameObject.SetActive(false);
                    }
                }
            }
        }
        //問題3
        if (this.mode == Mode.Question3)
        {
            if (yes.isOn == true)
            {
                toggle_flag = 1;
                material9.gameObject.SetActive(true);
                material9.color = new Color32(50, 50, 50, 100);
            }
            if (no.isOn == true)
            {
                toggle_flag = 2;
                material10.gameObject.SetActive(true);
                material10.color = new Color32(50, 50, 50, 100);
            }
            if(etc.isOn == true)
            {
                toggle_flag = 3;
                material11.gameObject.SetActive(true);
                material11.color = new Color32(50, 50, 50, 100);
            }
            if (round != (FONT_COUNT - 1))
            {
                if (yes.isOn == true || no.isOn == true || etc.isOn == true)
                {
                    Question3_Next.gameObject.SetActive(true);
                }
                else
                {
                    Question3_Next.gameObject.SetActive(false);
                }
                if (back_flag == true)
                {
                    Question3_Back.gameObject.SetActive(false);
                }
                if (back_flag == false)
                {
                    Question3_Back.gameObject.SetActive(true);
                }
            }
            else
            {
                if(image_number != NUMBER_OF_QUESTION_3 - 1)
                {
                    if (yes.isOn == true || no.isOn == true || etc.isOn == true)
                    {
                        Question3_Next.gameObject.SetActive(true);
                    }
                    else
                    {
                       Question3_Next.gameObject.SetActive(false);
                    }
                    if (back_flag == true)
                    {
                        Question3_Back.gameObject.SetActive(false);
                    }
                    if (back_flag == false)
                    {
                        Question3_Back.gameObject.SetActive(true);
                    }
                }
                else
                {
                        End_Button.gameObject.SetActive(true);
                }
            }
        }

        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            sw.Close();
        }*/
    }

    //フォント変更関数
    private void FontManager()
    {
        if(this.mode == Mode.Question1)
        {
            if(font_number == 0)
            {
                answer1.font = Msgothic;
                answer2.font = Msgothic;
                answer3.font = Msgothic;
                answer4.font = Msgothic;
            }
            else if(font_number == 1)
            {
                answer1.font = Udd;
                answer2.font = Udd;
                answer3.font = Udd;
                answer4.font = Udd;
            }
            else if(font_number == 2)
            {
                answer1.font = Yumin;
                answer2.font = Yumin;
                answer3.font = Yumin;
                answer4.font = Yumin;
            }

        }
        else if (this.mode == Mode.Question2)
        {
            if (font_number == 0)
            {
                question2_japan.font = Msgothic;
                answer5.font = Msgothic;
                answer6.font = Msgothic;
                answer7.font = Msgothic;
                answer8.font = Msgothic;
            }
            else if (font_number == 1)
            {
                question2_japan.font= Udd;
                answer5.font = Udd;
                answer6.font = Udd;
                answer7.font = Udd;
                answer8.font = Udd;
            }
            else if (font_number == 2)
            {
                question2_japan.font = Yumin;
                answer5.font = Yumin;
                answer6.font = Yumin;
                answer7.font = Yumin;
                answer8.font = Yumin;
            }
        }
        else if (this.mode == Mode.Question3)
        {
            if (font_number== 0)
            {
               question3.font = Msgothic;
            }
            else if (font_number == 1)
            {
                question3.font = Udd;
            }
            else if (font_number == 2)
            {
                question3.font = Yumin;
            }

        }
    }

    //全トグルスイッチたちOFF関数
    public void SetAllTogglesOff()
    {
        con_toggle1.isOn = false;
        con_toggle2.isOn = false;
        con_toggle3.isOn = false;
        con_toggle4.isOn = false;
        con_toggle5.isOn = false;
        con_toggle6.isOn = false;
        understandToggle.isOn = false;

        understandToggle.isOn = false;

        japan_agree.isOn = false;
        

        japan_disagree.isOn = false;

        japan_etc.isOn = false;

        ageYes.isOn = false;
        ageNo.isOn = false;
        consentYes.isOn = false;
        consentNo.isOn = false;

        data_toggle1.isOn = false;
        data_toggle2.isOn = false;
        data_toggle3.isOn = false;
        data_toggle4.isOn = false;
        data_toggle5.isOn = false;
        data_toggle6.isOn = false;
        data_toggle7.isOn = false;
        data_toggle8.isOn = false;
        data_toggle9.isOn = false;
        data_toggle10.isOn = false;
        data_toggle11.isOn = false;
        data_toggle12.isOn = false;
        data_toggle13.isOn = false;
        data_toggle14.isOn = false;
        data_toggle15.isOn = false;
        data_toggle16.isOn = false;
        data_toggle17.isOn = false;
        data_toggle18.isOn = false;
        data_toggle19.isOn = false;
        data_toggle20.isOn = false;
        data_toggle21.isOn = false;

        data_toggle_language1.isOn = false;
        data_toggle_language2.isOn = false;
        data_toggle_language3.isOn = false;
        data_toggle_language4.isOn = false;
        data_toggle_language5.isOn = false;
        data_toggle_language6.isOn = false;
        data_toggle_language7.isOn = false;
        data_toggle_language8.isOn = false;
        data_toggle_language9.isOn = false;
        data_toggle_language10.isOn = false;
        data_toggle_language11.isOn = false;
        data_toggle_language12.isOn = false;
        data_toggle_language13.isOn = false;
        data_toggle_language14.isOn = false;
        data_toggle_language15.isOn = false;
        data_toggle_language16.isOn = false;

        toggle1.isOn = false;
        toggle2.isOn = false;
        toggle3.isOn = false;
        toggle4.isOn = false;
        toggle5.isOn = false;
        toggle6.isOn = false;
        toggle7.isOn = false;
        toggle8.isOn = false;

        toggle_other.isOn = false;
        yes.isOn = false;
        no.isOn = false;
        etc.isOn = false;
    }

    //アクティブ化OFF
    private void NotActiveObjects()
    {
        sign.gameObject.SetActive(false);
        reason.gameObject.SetActive(false);
        month.gameObject.SetActive(false);
        date.gameObject.SetActive(false);
        id.gameObject.SetActive(false); 
        language.gameObject.SetActive(false);

        con_toggle1.gameObject.SetActive(false);
        con_toggle2.gameObject.SetActive(false);
        con_toggle3.gameObject.SetActive(false);
        con_toggle4.gameObject.SetActive(false);
        con_toggle5.gameObject.SetActive(false);
        con_toggle6.gameObject.SetActive(false);
        understandToggle.gameObject.SetActive(false);   
        japan_agree.gameObject.SetActive(false);
        japan_disagree.gameObject.SetActive(false);
        japan_etc.gameObject.SetActive(false);

        ageYes.gameObject.SetActive(false);
        ageNo.gameObject.SetActive(false);
        consentYes.gameObject.SetActive(false);
        consentNo.gameObject.SetActive(false);

        con_material1.gameObject.SetActive(false);
        con_material2.gameObject.SetActive(false);
        con_material3.gameObject.SetActive(false);
        con_material4.gameObject.SetActive(false);
        con_material5.gameObject.SetActive(false);
        con_material6.gameObject.SetActive(false);
        understandMate.gameObject.SetActive(false);
        con_material8.gameObject.SetActive(false);
        con_material9.gameObject.SetActive(false);
        con_material10.gameObject.SetActive(false);
        con_material11.gameObject.SetActive(false);
        con_material12.gameObject.SetActive(false);

        ageYes_material.gameObject.SetActive(false);
        ageYes_mane.gameObject.SetActive(false);
        ageNo_material.gameObject.SetActive(false);
        ageNo_mane.gameObject.SetActive(false);

        consentYes_material.gameObject.SetActive(false);
        consentYes_mane.gameObject.SetActive(false);
        consentNo_material.gameObject.SetActive(false);
        consentNo_mane.gameObject.SetActive(false);


        con_mateJapan_agree.gameObject.SetActive(false);
        con_mateNihon_agree.gameObject.SetActive(false);
        con_mateEnglish_agree.gameObject.SetActive(false);
        con_mateChina_agree.gameObject.SetActive(false);
        con_mateBeto_agree.gameObject.SetActive(false);
        con_mateMyan_agree.gameObject.SetActive(false);

        con_mateJapan_disagree.gameObject.SetActive(false);
        con_mateNihon_disagree.gameObject.SetActive(false);
        con_mateEnglish_disagree.gameObject.SetActive(false);
        con_mateChina_disagree.gameObject.SetActive(false);
        con_mateBeto_disagree.gameObject.SetActive(false);
        con_mateMyan_disagree.gameObject.SetActive(false);

        con_mateJapan_etc.gameObject.SetActive(false);
        con_mateNihon_etc.gameObject.SetActive(false);
        con_mateEnglish_etc.gameObject.SetActive(false);
        con_mateChina_etc.gameObject.SetActive(false);
        con_mateBeto_etc.gameObject.SetActive(false);
        con_mateMyan_etc.gameObject.SetActive(false);

        con_manage1.gameObject.SetActive(false);
        con_manage2.gameObject.SetActive(false);
        con_manage3.gameObject.SetActive(false);
        con_manage4.gameObject.SetActive(false);
        con_manage5.gameObject.SetActive(false);
        con_manage6.gameObject.SetActive(false);
        understandManage.gameObject.SetActive(false);
       
        con_maneJapan_agree.gameObject.SetActive(false);
        con_maneNihon_agree.gameObject.SetActive(false);
        con_maneEnglish_agree.gameObject.SetActive(false);
        con_maneChina_agree.gameObject.SetActive(false);
        con_maneBeto_agree.gameObject.SetActive(false);
        con_maneMyan_agree.gameObject.SetActive(false);

        con_maneJapan_disagree.gameObject.SetActive(false);
        con_maneNihon_disagree.gameObject.SetActive(false);
        con_maneEnglish_disagree.gameObject.SetActive(false);
        con_maneChina_disagree.gameObject.SetActive(false);
        con_maneBeto_disagree.gameObject.SetActive(false);
        con_maneMyan_disagree.gameObject.SetActive(false);

        con_maneJapan_etc.gameObject.SetActive(false);
        con_maneNihon_etc.gameObject.SetActive(false);
        con_maneEnglish_etc.gameObject.SetActive(false);
        con_maneChina_etc.gameObject.SetActive(false);
        con_maneBeto_etc.gameObject.SetActive(false);
        con_maneMyan_etc.gameObject.SetActive(false);

        data_toggle1.gameObject.SetActive (false);
        data_toggle2.gameObject.SetActive(false);
        data_toggle3.gameObject.SetActive(false);
        data_toggle4.gameObject.SetActive(false);
        data_toggle5.gameObject.SetActive(false);
        data_toggle6.gameObject.SetActive(false);
        data_toggle7.gameObject.SetActive(false);
        data_toggle8.gameObject.SetActive(false);
        data_toggle9.gameObject.SetActive(false);
        data_toggle10.gameObject.SetActive(false);
        data_toggle11.gameObject.SetActive(false);
        data_toggle12.gameObject.SetActive(false);
        data_toggle13.gameObject.SetActive(false);
        data_toggle14.gameObject.SetActive(false);
        data_toggle15.gameObject.SetActive(false);
        data_toggle16.gameObject.SetActive(false);
        data_toggle17.gameObject.SetActive(false);
        data_toggle18.gameObject.SetActive(false);
        data_toggle19.gameObject.SetActive(false);
        data_toggle20.gameObject.SetActive(false);
        data_toggle21.gameObject.SetActive(false);

        data_toggle_language1.gameObject.SetActive(false);
        data_toggle_language2.gameObject.SetActive(false);
        data_toggle_language3.gameObject.SetActive(false);
        data_toggle_language4.gameObject.SetActive(false);
        data_toggle_language5.gameObject.SetActive(false);
        data_toggle_language6.gameObject.SetActive(false);
        data_toggle_language7.gameObject.SetActive(false);
        data_toggle_language8.gameObject.SetActive(false);
        data_toggle_language9.gameObject.SetActive(false);
        data_toggle_language10.gameObject.SetActive(false);
        data_toggle_language11.gameObject.SetActive(false);
        data_toggle_language12.gameObject.SetActive(false);
        data_toggle_language13.gameObject.SetActive(false);
        data_toggle_language14.gameObject.SetActive(false);
        data_toggle_language15.gameObject.SetActive(false);
        data_toggle_language16.gameObject.SetActive(false);

        data_material1.gameObject.SetActive(false);
        data_material2.gameObject.SetActive(false);
        data_material3.gameObject.SetActive(false);
        data_material4.gameObject.SetActive(false);
        data_material5.gameObject.SetActive(false);
        data_material6.gameObject.SetActive(false);
        data_material7.gameObject.SetActive(false);
        data_material8.gameObject.SetActive(false);
        data_material9.gameObject.SetActive(false);
        data_material10.gameObject.SetActive(false);
        data_material11.gameObject.SetActive(false);
        data_material12.gameObject.SetActive(false);
        data_material13.gameObject.SetActive(false);
        data_material14.gameObject.SetActive(false);
        data_material15.gameObject.SetActive(false);
        data_material16.gameObject.SetActive(false);
        data_material17.gameObject.SetActive(false);
        data_material18.gameObject.SetActive(false);
        data_material19.gameObject.SetActive(false);
        data_material20.gameObject.SetActive(false);
        data_material21.gameObject.SetActive(false);

        data_material_Language1.gameObject.SetActive(false);
        data_material_Language2.gameObject.SetActive(false);
        data_material_Language3.gameObject.SetActive(false);
        data_material_Language4.gameObject.SetActive(false);
        data_material_Language5.gameObject.SetActive(false);
        data_material_Language6.gameObject.SetActive(false);
        data_material_Language7.gameObject.SetActive(false);
        data_material_Language8.gameObject.SetActive(false);
        data_material_Language9.gameObject.SetActive(false);
        data_material_Language10.gameObject.SetActive(false);
        data_material_Language11.gameObject.SetActive(false);
        data_material_Language12.gameObject.SetActive(false);
        data_material_Language13.gameObject.SetActive(false);
        data_material_Language14.gameObject.SetActive(false);
        data_material_Language15.gameObject.SetActive(false);
        data_material_Language16.gameObject.SetActive(false);


        data_manage1.gameObject.SetActive(false);
        data_manage2.gameObject.SetActive(false);
        data_manage3.gameObject.SetActive(false);
        data_manage4.gameObject.SetActive(false);
        data_manage5.gameObject.SetActive(false);
        data_manage6.gameObject.SetActive(false);
        data_manage7.gameObject.SetActive(false);
        data_manage8.gameObject.SetActive(false);
        data_manage9.gameObject.SetActive(false);
        data_manage10.gameObject.SetActive(false);
        data_manage11.gameObject.SetActive(false);
        data_manage12.gameObject.SetActive(false);
        data_manage13.gameObject.SetActive(false);
        data_manage14.gameObject.SetActive(false);
        data_manage15.gameObject.SetActive(false);
        data_manage16.gameObject.SetActive(false);
        data_manage17.gameObject.SetActive(false);
        data_manage18.gameObject.SetActive(false);
        data_manage19.gameObject.SetActive(false);
        data_manage20.gameObject.SetActive(false);
        data_manage21.gameObject.SetActive(false);

        data_manage_Language1.gameObject.SetActive(false);
        data_manage_Language2.gameObject.SetActive(false);
        data_manage_Language3.gameObject.SetActive(false);
        data_manage_Language4.gameObject.SetActive(false);
        data_manage_Language5.gameObject.SetActive(false);
        data_manage_Language6.gameObject.SetActive(false);
        data_manage_Language7.gameObject.SetActive(false);
        data_manage_Language8.gameObject.SetActive(false);
        data_manage_Language9.gameObject.SetActive(false);
        data_manage_Language10.gameObject.SetActive(false);
        data_manage_Language11.gameObject.SetActive(false);
        data_manage_Language12.gameObject.SetActive(false);
        data_manage_Language13.gameObject.SetActive(false);
        data_manage_Language14.gameObject.SetActive(false);
        data_manage_Language15.gameObject.SetActive(false);
        data_manage_Language16.gameObject.SetActive(false);

        Consent_Next.gameObject.SetActive (false);
        Data_Next.gameObject .SetActive (false);
        Question1_Next.gameObject .SetActive (false);
        Question2_Next.gameObject .SetActive (false);
        Question3_Next.gameObject .SetActive (false);
        Data_Next.gameObject.SetActive(false);
        Question1_Back.gameObject.SetActive (false);
        Question2_Back.gameObject.SetActive (false);
        Question1_Change.gameObject.SetActive (false);
        Question2_Change.gameObject.SetActive (false);
        Consent_Change_Button.gameObject.SetActive(false);
        backTitle_Button.gameObject.SetActive(false);
        Data_Change_Button.gameObject.SetActive(false);
    }

    //各問題のタイトル表示関数
    private void TitleManager()
    {
        if (title_number == 0)
        {
            title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\main");
            Set_TextImage_Title(title_images[0]);
            title_image.sprite = title_images[0]; //問題画像の貼り換え
        }
        else if (title_number == 1)
        {
            if (Language == Selects.Japan || Language == Selects.Nihon)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\japan");
                Set_TextImage_Title(title_images[0]);
                title_image.sprite = title_images[0]; //問題画像の貼り換え
            }
            else if (Language == Selects.English)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\english");
                Set_TextImage_Title(title_images[0]);
                title_image.sprite = title_images[0]; //問題画像の貼り換え
            }
            else if (Language == Selects.China)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\china");
                Set_TextImage_Title(title_images[0]);
                title_image.sprite = title_images[0]; //問題画像の貼り換え
            }
            else if (Language == Selects.Beto)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\beto");
                Set_TextImage_Title(title_images[0]);
                title_image.sprite = title_images[0]; //問題画像の貼り換え
            }
            else if (Language == Selects.Myan)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\myan");
                Set_TextImage_Title(title_images[0]);
                title_image.sprite = title_images[0]; //問題画像の貼り換え
            }
        }
        else if (title_number == 2)
        {
            title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\main");
            Set_TextImage_Title(title_images[1]);
            title_image.sprite = title_images[1]; //問題画像の貼り換え
        }
        else if(title_number == 3)
        {
            if (Language == Selects.Japan || Language == Selects.Nihon)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\japan");
                Set_TextImage_Title(title_images[1]);
                title_image.sprite = title_images[1]; //問題画像の貼り換え
            }
            else if (Language == Selects.English)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\english");
                Set_TextImage_Title(title_images[1]);
                title_image.sprite = title_images[1]; //問題画像の貼り換え
            }
            else if (Language == Selects.China)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\china");
                Set_TextImage_Title(title_images[1]);
                title_image.sprite = title_images[1]; //問題画像の貼り換え
            }
            else if (Language == Selects.Beto)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\beto");
                Set_TextImage_Title(title_images[1]);
                title_image.sprite = title_images[1]; //問題画像の貼り換え
            }
            else if (Language == Selects.Myan)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\myan");
                Set_TextImage_Title(title_images[1]);
                title_image.sprite = title_images[1]; //問題画像の貼り換え
            }
        }
        else if (title_number == 4)
        {
            title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\main");
            Set_TextImage_Title(title_images[2]);
            title_image.sprite = title_images[2]; //問題画像の貼り換え
        }
        else if (title_number == 5)
        {
            if (Language == Selects.Japan || Language == Selects.Nihon)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\japan");
                Set_TextImage_Title(title_images[2]);
                title_image.sprite = title_images[2]; //問題画像の貼り換え
            }
            else if (Language == Selects.English)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\english");
                Set_TextImage_Title(title_images[2]);
                title_image.sprite = title_images[2]; //問題画像の貼り換え
            }
            else if (Language == Selects.China)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\china");
                Set_TextImage_Title(title_images[2]);
                title_image.sprite = title_images[2]; //問題画像の貼り換え
            }
            else if (Language == Selects.Beto)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\beto");
                Set_TextImage_Title(title_images[2]);
                title_image.sprite = title_images[2]; //問題画像の貼り換え
            }
            else if (Language == Selects.Myan)
            {
                title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\myan");
                Set_TextImage_Title(title_images[2]);
                title_image.sprite = title_images[2]; //問題画像の貼り換え
            }
        }
        else if (title_number == 6)
        {
            title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\main");
            Set_TextImage_Title(title_images[3]);
            title_image.sprite = title_images[3]; //問題画像の貼り換え
        }
        else if (title_number == 7)
        {
            title_images = Resources.LoadAll<Sprite>("nagasaki-GameTitle\\main");
            Set_TextImage_Title(title_images[4]);
            title_image.sprite = title_images[4]; //問題画像の貼り換え
        }
    }

    //画像と解答変更関数
    private void ImageManager()
    {
        if(this.mode == Mode.Consent_Main)
        {
            if (selects == Selects.Japan)
            {
                sprites = Resources.LoadAll<Sprite>("Consent\\japan");
                Set_TextImage_Consent(sprites[language_number]);
                consent_image.sprite = sprites[language_number];
            }
            else if (selects == Selects.Nihon)
            {
                sprites = Resources.LoadAll<Sprite>("Consent\\nihon");
                Set_TextImage_Consent(sprites[language_number]);
                consent_image.sprite = sprites[language_number];
            }
            else if (selects == Selects.English)
            {
                sprites = Resources.LoadAll<Sprite>("Consent\\eng");
                Set_TextImage_Consent(sprites[language_number]);
                consent_image.sprite = sprites[language_number];
            }
            else if (selects == Selects.China)
            {
                sprites = Resources.LoadAll<Sprite>("Consent\\china");
                Set_TextImage_Consent(sprites[language_number]);
                consent_image.sprite = sprites[language_number];
            }
            else if (selects == Selects.Beto)
            {
                sprites = Resources.LoadAll<Sprite>("Consent\\beto");
                Set_TextImage_Consent(sprites[language_number]);
                consent_image.sprite = sprites[language_number];
            }
            else if (selects == Selects.Myan)
            {
                sprites = Resources.LoadAll<Sprite>("Consent\\myan");
                Set_TextImage_Consent(sprites[language_number]);
                consent_image.sprite = sprites[language_number];
            }
            else
            {
                sprites = Resources.LoadAll<Sprite>("Consent\\nagasaki-Consentform");
                Set_TextImage_Consent(sprites[main_number]);
                consent_image.sprite = sprites[main_number];
            }
        }

        if (this.mode == Mode.Data)
        {
            if (Language == Selects.Japan)
            {
                data_images = Resources.LoadAll<Sprite>("data-paper\\japan");
                Set_TextImage_Data(data_images[image_number]);
                data_image.sprite = data_images[image_number];
            }
            else if (Language == Selects.Nihon)
            {
                data_images = Resources.LoadAll<Sprite>("data-paper\\nihon");
                Set_TextImage_Data(data_images[image_number]);
                data_image.sprite = data_images[image_number];
            }
            else if (Language == Selects.English)
            {
                data_images = Resources.LoadAll<Sprite>("data-paper\\english");
                Set_TextImage_Data(data_images[image_number]);
                data_image.sprite = data_images[image_number];
            }
            else if (Language == Selects.China)
            {
                data_images = Resources.LoadAll<Sprite>("data-paper\\china");
                Set_TextImage_Data(data_images[image_number]);
                data_image.sprite = data_images[image_number];
            }
            else if (Language == Selects.Beto)
            {
                data_images = Resources.LoadAll<Sprite>("data-paper\\beto");
                Set_TextImage_Data(data_images[image_number]);
                data_image.sprite = data_images[image_number];
            }
            else if (Language == Selects.Myan)
            {
                data_images = Resources.LoadAll<Sprite>("data-paper\\myan");
                Set_TextImage_Data(data_images[image_number]);
                data_image.sprite = data_images[image_number];
            }

        }

        else if (this.mode == Mode.Question1)
        {
            font_number = FontArray[csv_number, round];
            //font_number = 0;
            if (font_number == 0)
            {
                firtstQuestionOther = Resources.LoadAll<Sprite>("question1\\Other");
                questionText_other.sprite = firtstQuestionOther[0];
                if (csv_number == 0)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\1");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 1)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\2");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 2)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\3");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 3)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\4");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 4)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\5");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 5)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\6");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 6)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\7");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 7)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\8");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 8)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\9");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 9)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\10");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 10)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\11");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 11)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\12");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 12)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\13");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 13)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\14");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 14)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\15");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 15)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\16");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 16)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\17");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 17)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\18");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 18)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\19");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 19)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\PGK\\20");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
            }
            else if (font_number == 1)
            {
                firtstQuestionOther = Resources.LoadAll<Sprite>("question1\\Other");
                questionText_other.sprite = firtstQuestionOther[1];
                if (csv_number == 0)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\1");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 1)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\2");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 2)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\3");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 3)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\4");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 4)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\5");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 5)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\6");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 6)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\7");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 7)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\8");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 8)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\9");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 9)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\10");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 10)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\11");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 11)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\12");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 12)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\13");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 13)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\14");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 14)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\15");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 15)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\16");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 16)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\17");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 17)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\18");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 18)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\19");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 19)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\UDD\\20");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
            }
            else if (font_number == 2)
            {
                firtstQuestionOther = Resources.LoadAll<Sprite>("question1\\Other");
                questionText_other.sprite = firtstQuestionOther[2];
                if (csv_number == 0)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\1");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 1)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\2");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 2)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\3");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 3)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\4");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 4)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\5");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 5)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\6");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 6)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\7");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 7)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\8");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 8)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\9");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 9)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\10");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 10)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\11");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 11)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\12");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 12)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\13");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 13)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\14");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 14)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\15");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 15)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\16");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 16)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\17");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 17)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\18");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 18)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\19");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }
                else if (csv_number == 19)
                {
                    firstQuestion = Resources.LoadAll<Sprite>("question1\\YMC\\20");
                    questionText_1_1.sprite = firstQuestion[csv_answers_num1[round, csv_number, 0]];
                    questionText_1_2.sprite = firstQuestion[csv_answers_num1[round, csv_number, 1]];
                    questionText_1_3.sprite = firstQuestion[csv_answers_num1[round, csv_number, 2]];
                    questionText_1_4.sprite = firstQuestion[csv_answers_num1[round, csv_number, 3]];
                }

            }
            //FontManager();
            Set_TextImage_Question1(first_images[csv_number]);
            question_image.sprite = first_images[csv_number]; //問題画像の貼り換え
        }

        else if (this.mode == Mode.Question2)
        {
            font_number = FontArray[csv_number, round];
            if (font_number == 0)
            {
                if (csv_number == 0)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\PGK\\1");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\PGK\\1");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(-20, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 1)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\PGK\\2");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\PGK\\2");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(85, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 2)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\PGK\\3");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\PGK\\3");
                    var questionPos = new Vector3(40, 330, 0);
                    var englishPos = new Vector3(100, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 3)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\PGK\\4");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\PGK\\4");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(35, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 4)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\PGK\\5");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\PGK\\5");
                    var questionPos = new Vector3(50, 330, 0);
                    var englishPos = new Vector3(100, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 5)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\PGK\\6");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\PGK\\6");
                    var questionPos = new Vector3(120, 330, 0);
                    var englishPos = new Vector3(50, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 6)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\PGK\\7");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\PGK\\7");
                    var questionPos = new Vector3(140, 330, 0);
                    var englishPos = new Vector3(50, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
            }
            else if (font_number == 1)
            {
                if (csv_number == 0)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\UDD\\1");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\UDD\\1");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(-20, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 1)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\UDD\\2");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\UDD\\2");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(85, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 2)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\UDD\\3");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\UDD\\3");
                    var questionPos = new Vector3(40, 330, 0);
                    var englishPos = new Vector3(100, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 3)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\UDD\\4");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\UDD\\4");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(35, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 4)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\UDD\\5");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\UDD\\5");
                    var questionPos = new Vector3(50, 330, 0);
                    var englishPos = new Vector3(100, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 5)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\UDD\\6");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\UDD\\6");
                    var questionPos = new Vector3(120, 330, 0);
                    var englishPos = new Vector3(50, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 6)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\UDD\\7");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\UDD\\7");
                    var questionPos = new Vector3(140, 330, 0);
                    var englishPos = new Vector3(50, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
            }
            else if (font_number == 2)
            {
                if (csv_number == 0)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\YMC\\1");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\YMC\\1");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(-20, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 1)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\YMC\\2");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\YMC\\2");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(85, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 2)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\YMC\\3");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\YMC\\3");
                    var questionPos = new Vector3(40, 330, 0);
                    var englishPos = new Vector3(100, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 3)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\YMC\\4");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\YMC\\4");
                    var questionPos = new Vector3(60, 330, 0);
                    var englishPos = new Vector3(35, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 4)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\YMC\\5");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\YMC\\5");
                    var questionPos = new Vector3(50, 330, 0);
                    var englishPos = new Vector3(100, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 5)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\YMC\\6");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\YMC\\6");
                    var questionPos = new Vector3(120, 330, 0);
                    var englishPos = new Vector3(50, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
                else if (csv_number == 6)
                {
                    secondQuestion = Resources.LoadAll<Sprite>("question2_answer\\YMC\\7");
                    questionText_2_1.sprite = secondQuestion[csv_answers_num2[round, csv_number, 0]];
                    questionText_2_2.sprite = secondQuestion[csv_answers_num2[round, csv_number, 1]];
                    questionText_2_3.sprite = secondQuestion[csv_answers_num2[round, csv_number, 2]];
                    questionText_2_4.sprite = secondQuestion[csv_answers_num2[round, csv_number, 3]];

                    second_images = Resources.LoadAll<Sprite>("question2_text\\YMC\\7");
                    var questionPos = new Vector3(140, 330, 0);
                    var englishPos = new Vector3(50, 230, 0);
                    questionImage.transform.localPosition = questionPos;
                    englishImage.transform.localPosition = englishPos;
                    questionImage.sprite = second_images[0];
                    englishImage.sprite = second_images[1];
                }
            }
            //FontManager();
            //question2_japan.text = question_Text[csv_number].Japan;
            //question2_english.text = question_Text[csv_number].English;
        }
        else if (this.mode == Mode.Question3)
        {
            font_number = FontArray[csv_number, round];
            if (font_number == 0)
            {
                third_images = Resources.LoadAll<Sprite>("question3\\PGK");
                questionImage3.sprite = third_images[csv_number];

                thirdQuestion = Resources.LoadAll<Sprite>("question3_answer\\PGK");
                noImage.sprite = thirdQuestion[0];
                yesImage.sprite = thirdQuestion[1];
                etcImage.sprite = thirdQuestion[2];

                if (csv_number == 0)
                {
                    var questionPos = new Vector3(200, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 1)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 2)
                {
                    var questionPos = new Vector3(190, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 3)
                {
                    var questionPos = new Vector3(80, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 4)
                {
                    var questionPos = new Vector3(170, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 5)
                {
                    var questionPos = new Vector3(10, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 6)
                {
                    var questionPos = new Vector3(60, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 7)
                {
                    var questionPos = new Vector3(165, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 8)
                {
                    var questionPos = new Vector3(15, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 9)
                {
                    var questionPos = new Vector3(30, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 10)
                {
                    var questionPos = new Vector3(40, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 11)
                {
                    var questionPos = new Vector3(30, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 12)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 13)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 14)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 15)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 16)
                {
                    var questionPos = new Vector3(90, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 17)
                {
                    var questionPos = new Vector3(140, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 18)
                {
                    var questionPos = new Vector3(120, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 19)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 20)
                {
                    var questionPos = new Vector3(230, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
            }
            else if(font_number == 1)
            {
                third_images = Resources.LoadAll<Sprite>("question3\\UDD");
                questionImage3.sprite = third_images[csv_number];

                thirdQuestion = Resources.LoadAll<Sprite>("question3_answer\\UDD");
                noImage.sprite = thirdQuestion[0];
                yesImage.sprite = thirdQuestion[1];
                etcImage.sprite = thirdQuestion[2];
                if (csv_number == 0)
                {
                    var questionPos = new Vector3(200, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 1)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 2)
                {
                    var questionPos = new Vector3(190, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 3)
                {
                    var questionPos = new Vector3(80, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 4)
                {
                    var questionPos = new Vector3(170, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 5)
                {
                    var questionPos = new Vector3(10, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 6)
                {
                    var questionPos = new Vector3(60, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 7)
                {
                    var questionPos = new Vector3(165, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 8)
                {
                    var questionPos = new Vector3(15, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 9)
                {
                    var questionPos = new Vector3(30, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 10)
                {
                    var questionPos = new Vector3(40, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 11)
                {
                    var questionPos = new Vector3(30, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 12)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 13)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 14)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 15)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 16)
                {
                    var questionPos = new Vector3(90, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 17)
                {
                    var questionPos = new Vector3(140, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 18)
                {
                    var questionPos = new Vector3(120, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 19)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 20)
                {
                    var questionPos = new Vector3(230, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
            }
            else if (font_number == 2)
            {
                third_images = Resources.LoadAll<Sprite>("question3\\YMC");
                questionImage3.sprite = third_images[csv_number];

                thirdQuestion = Resources.LoadAll<Sprite>("question3_answer\\YMC");
                noImage.sprite = thirdQuestion[0];
                yesImage.sprite = thirdQuestion[1];
                etcImage.sprite = thirdQuestion[2];

                if (csv_number == 0)
                {
                    var questionPos = new Vector3(200, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 1)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 2)
                {
                    var questionPos = new Vector3(190, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 3)
                {
                    var questionPos = new Vector3(80, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 4)
                {
                    var questionPos = new Vector3(170, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 5)
                {
                    var questionPos = new Vector3(10, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 6)
                {
                    var questionPos = new Vector3(60, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 7)
                {
                    var questionPos = new Vector3(165, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 8)
                {
                    var questionPos = new Vector3(15, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 9)
                {
                    var questionPos = new Vector3(30, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 10)
                {
                    var questionPos = new Vector3(40, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 11)
                {
                    var questionPos = new Vector3(30, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 12)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 13)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 14)
                {
                    var questionPos = new Vector3(20, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 15)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 16)
                {
                    var questionPos = new Vector3(90, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 17)
                {
                    var questionPos = new Vector3(140, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 18)
                {
                    var questionPos = new Vector3(120, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 19)
                {
                    var questionPos = new Vector3(5, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }
                else if (csv_number == 20)
                {
                    var questionPos = new Vector3(230, 335, 0);
                    questionImage3.transform.localPosition = questionPos;
                }

            }
        }
    }

    //問題キャンバス切り替え関数
    public void CanvasManager()
    {
        consent.gameObject.SetActive(false);
        data_canvas.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        canvas1.gameObject.SetActive(false);
        canvas2.gameObject.SetActive(false);
        canvas3.gameObject.SetActive(false);
        switch (mode)
        {
            case Mode.Consent_Main:
                consent.gameObject.SetActive(true);
                NotActiveObjects();
                ImageManager();

                Vector3 pos = new Vector3(X2, Y2, Z);
                pos.x = X2;
                pos.y = Y2;
                Consent_Next.transform.localPosition = pos;
                break;

            case Mode.Data:
                data_canvas.gameObject.SetActive(true);
                NotActiveObjects();
                ImageManager();

                Vector3 dataPos = new Vector3(X2, Y2, Z2);
                dataPos.x = X2;    
                dataPos.y = Y2;
                Data_Next.transform.localPosition = dataPos;
                break;


            case Mode.Title: //各問題のtitleを表示
                if (title_number == 0)
                {
                    Vector3 titlePos = new Vector3(0,-300,0);
                    title_image.transform.localPosition = titlePos;
                    backTitle_Button2.gameObject.SetActive(false);
                    Title_Change_Button.gameObject.SetActive(true);
                    title.gameObject.SetActive(true);
                    TitleManager(); //各問題のタイトル表示
                    float width = 592;
                    float height = 1280;
                    title_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                }
                else if(title_number == 1)
                {
                    Vector3 titlePos = new Vector3(0, 0, 0);
                    title_image.transform.localPosition = titlePos;
                    backTitle_Button2.gameObject.SetActive(false);
                    title.gameObject.SetActive(true);
                    TitleManager(); //各問題のタイトル表示
                }
                else if (title_number == 3)
                {
                    title.gameObject.SetActive(true);
                    backTitle_Button2.gameObject.SetActive(false);
                    TitleManager(); //各問題のタイトル表示
                }

                else if (title_number == 5)
                {
                    title.gameObject.SetActive(true);
                    backTitle_Button2.gameObject.SetActive(false);
                    TitleManager(); //各問題のタイトル表示
                }
                break;

            case Mode.Question1: //各問題を表示
                Title_Change_Button.gameObject.SetActive(false);
                canvas1.gameObject.SetActive(true);
                Question1_Change.gameObject.SetActive(false);

                image_number = 0;
                round = 0;
                csv_number = Question_number_1[0,0];
                //csv_number = 0;

                ImageManager();

                Question.questionNumber = 1;
                this.dt = DateTime.Now;
                break;

            case Mode.Question2:
                Title_Change_Button.gameObject.SetActive(false);
                canvas2.gameObject.SetActive(true);
                Question2_Change.gameObject.SetActive(false);

                image_number = 0;
                round = 0;
                csv_number = Question_number_2[0, 0];

                ImageManager();
                Question.questionNumber = 2;
                this.dt = DateTime.Now;
                break;

            case Mode.Question3:
                Title_Change_Button.gameObject.SetActive(false);
                canvas3.gameObject.SetActive(true);
                End_Button.gameObject.SetActive(false);

                image_number = 0;
                round = 0;
                csv_number = Question_number_3[round, image_number];

                ImageManager();
                Question.questionNumber = 3;
                this.dt = DateTime.Now;
                break;
        }
    }

    //トグルマネージャー
    private void ToggleManager()
    {
        //問題1
        if (this.mode == Mode.Question1)
        {
            if (toggle_flag == 1)
            {
                toggle1.isOn = true;
            }
            if (toggle_flag == 2)
            {
                toggle2.isOn = true;
            }
            if (toggle_flag == 3)
            {
                toggle3.isOn = true;
            }
            if (toggle_flag == 4)
            {
                toggle4.isOn = true;
            }
            if (toggle_flag == 5)
            {
                toggle_other.isOn = true;
            }
        }
        //問題2
        else if(this.mode == Mode.Question2) 
        {
            if (toggle_flag == 1)
            {
                toggle5.isOn = true;
            }
            if (toggle_flag == 2)
            {
                toggle6.isOn = true;
            }
            if (toggle_flag == 3)
            {
                toggle7.isOn = true;
            }
            if (toggle_flag == 4)
            {
                toggle8.isOn = true;
            }
        }
        else if (this.mode == Mode.Question3)
        {
            if (toggle_flag == 1)
            {
               yes.isOn = true;
            }
            else if (toggle_flag == 2)
            {
                no.isOn = true;
            }
            else if (toggle_flag == 3)
            {
                etc.isOn = true;
            }
        }
    }

    private void GetUserAnswer()
    {
        if(this.mode == Mode.Data)
        {
            if(image_number == 2)
            {
                if(data_toggle_language1.isOn == true)
                {
                    Input_text[LANGUAGE] = "日本語";
                }
                else if (data_toggle_language2.isOn == true)
                {
                    Input_text[LANGUAGE] = "英語";
                }
                else if (data_toggle_language3.isOn == true)
                {
                    Input_text[LANGUAGE] = "中国語";
                }
                else if (data_toggle_language4.isOn == true)
                {
                    Input_text[LANGUAGE] = "韓国語";
                }
                else if (data_toggle_language5.isOn == true)
                {
                    Input_text[LANGUAGE] = "ベトナム語";
                }
                else if (data_toggle_language6.isOn == true)
                {
                    Input_text[LANGUAGE] = "タイ語";
                }
                else if (data_toggle_language7.isOn == true)
                {
                    Input_text[LANGUAGE] = "フランス語";
                }
                else if (data_toggle_language8.isOn == true)
                {
                    Input_text[LANGUAGE] = "タガログ語";
                }
                else if (data_toggle_language9.isOn == true)
                {
                    Input_text[LANGUAGE] = "ポルトガル語";
                }
                else if (data_toggle_language10.isOn == true)
                {
                    Input_text[LANGUAGE] = "スペイン語";
                }
                else if (data_toggle_language11.isOn == true)
                {
                    Input_text[LANGUAGE] = "ネパール語";
                }
                else if (data_toggle_language12.isOn == true)
                {
                    Input_text[LANGUAGE] = "インドネシア語";
                }
                else if (data_toggle_language13.isOn == true)
                {
                    Input_text[LANGUAGE] = "シンハラ語";
                }
                else if (data_toggle_language14.isOn == true)
                {
                    Input_text[LANGUAGE] = "ヒンディー語";
                }
                else if (data_toggle_language16.isOn == true)
                {
                    Input_text[LANGUAGE] = "ミャンマー語";
                }
                else if(data_toggle_language15 == true)
                {
                    Input_text[LANGUAGE] = language.text;
                }
            }
            if (image_number == 3)
            {
                if (data_toggle1.isOn == true)
                {
                    userChoice_data[0, 0] = "6か月未満";
                }
                else if (data_toggle2.isOn == true)
                {
                    userChoice_data[0, 1] = "6か月～1年未満";
                }
                else if (data_toggle3.isOn == true)
                {
                    userChoice_data[0, 2] = "1年～2年未満";
                }
                else if (data_toggle4.isOn == true)
                {
                    userChoice_data[0, 3] = "2年～3年未満";
                }
                else if (data_toggle5.isOn == true)
                {
                    userChoice_data[0, 4] = "3年以上";
                }
            }
            else if (image_number == 4)
            {
                if (data_toggle6.isOn == true)
                {
                    userChoice_data[1, 0] = "日本語ネイティブの先生";
                }
                if (data_toggle7.isOn == true)
                {
                    userChoice_data[1, 1] = "非日本語ネイティブの先生";
                }
                if (data_toggle8.isOn == true)
                {
                    userChoice_data[1, 2] = "ウェブサイト";
                }
                if (data_toggle9.isOn == true)
                {
                    userChoice_data[1, 3] = "母国で作られた教科書";
                }
                if (data_toggle10.isOn == true)
                {
                    userChoice_data[1, 4] = "日本で作られた教科書";
                }

            }
            else if (image_number == 5)
            {
                if (data_toggle11.isOn == true)
                {
                    userChoice_data[2, 0] = "N１";
                }
                else if (data_toggle12.isOn == true)
                {
                    userChoice_data[2, 1] = "N２";
                }
                else if (data_toggle13.isOn == true)
                {
                    userChoice_data[2, 2] = "N３";
                }
                else if (data_toggle14.isOn == true)
                {
                    userChoice_data[2, 3] = "N４";
                }
                else if (data_toggle15.isOn == true)
                {
                    userChoice_data[2, 4] = "N５";
                }
                else if (data_toggle16.isOn == true)
                {
                    userChoice_data[2, 5] = "ない";
                }
            }
            else if (image_number == 6)
            {
                if (data_toggle17.isOn == true)
                {
                    userChoice_data[3, 0] = "12歳以下";
                }
                else if (data_toggle18.isOn == true)
                {
                    userChoice_data[3, 1] = "12歳～18歳";
                }
                else if (data_toggle19.isOn == true)
                {
                    userChoice_data[3, 2] = "19歳～29歳";
                }
                else if (data_toggle20.isOn == true)
                {
                    userChoice_data[3, 3] = "30代";
                }
                else if (data_toggle21.isOn == true)
                {
                    userChoice_data[3, 4] = "40代以上";
                }
            }
        }
        else if(this.mode == Mode.Question1)
        {
            if(toggle1.isOn == true)
            {
                Question.userAnswer = Csv_answers_1[Question_number_1[round, image_number], csv_answers_num1[round, Question_number_1[round, image_number], 0]];
            }
            else if (toggle2.isOn == true)
            {
                Question.userAnswer = Csv_answers_1[Question_number_1[round, image_number], csv_answers_num1[round, Question_number_1[round, image_number], 1]];
            }
            else if (toggle3.isOn == true)
            {
                Question.userAnswer = Csv_answers_1[Question_number_1[round, image_number], csv_answers_num1[round, Question_number_1[round, image_number], 2]];
            }
            else if (toggle4.isOn == true)
            {
                Question.userAnswer = Csv_answers_1[Question_number_1[round, image_number], csv_answers_num1[round, Question_number_1[round, image_number], 3]];
            }
            else if (toggle_other.isOn == true)
            {
                Question.userAnswer = "わかりません";
            }
        }
        else if (this.mode == Mode.Question2)
        {
            if (toggle5.isOn == true)
            {
                Question.userAnswer = Csv_answers_2[Question_number_2[round, image_number], csv_answers_num2[round, Question_number_2[round, image_number], 0]];
            }
            else if (toggle6.isOn == true)
            {
                Question.userAnswer = Csv_answers_2[Question_number_2[round, image_number], csv_answers_num2[round, Question_number_2[round, image_number], 1]];
            }
            else if (toggle7.isOn == true)
            {
                Question.userAnswer = Csv_answers_2[Question_number_2[round, image_number], csv_answers_num2[round, Question_number_2[round, image_number], 2]];
            }
            else if (toggle8.isOn == true)
            {
                Question.userAnswer = Csv_answers_2[Question_number_2[round, image_number], csv_answers_num2[round, Question_number_2[round, image_number], 3]];
            }
        }
        else if(this.mode == Mode.Question3)
        {
            if(yes.isOn == true)
            {
                Question.userAnswer = "はい";
            }
            else if(no.isOn == true)
            {
                Question.userAnswer = "いいえ";
            }
            else if (etc.isOn == true)
            {
                Question.userAnswer = "わかりません";
            }
        }
    }

    private void GetUserFont()
    {
        if (font_number == 0)
        {
            Question.font = "Msgothic";
        }
        else if (font_number == 1)
        {
            Question.font = "Udd";
        }
        else
        {
            Question.font = "Yumin";
        }
    }

    private void TimeConverter_Responce()
    {
        Num = ResponceTime;
        float floatTimeSpan;
        int seconds, milliseconds;
        seconds = Num.Seconds;
        milliseconds = Num.Milliseconds;
        floatTimeSpan = (float)seconds + ((float)milliseconds / 1000);
        Question.responseTime = floatTimeSpan + responce;
    }
    private void TimeConverter_Back()
    {
        if(back_flag == false)
        {
            Question.backTime = 0;
        }
        else
        {
            Num = backTime;
            float floatTimeSpan;
            int seconds, milliseconds;
            seconds = Num.Seconds;
            milliseconds = Num.Milliseconds;
            floatTimeSpan = (float)seconds + ((float)milliseconds / 1000);
            Question.backTime = floatTimeSpan;
        }
    }

    //メインネクスト関数
    public void Main_Next()
    {
        if (this.mode == Mode.Consent_Main)
        {
            if (image_number == 0) //同意画面のタイトル画面
            {
                Vector3 pos = new Vector3(X2, Y2, Z);
                pos.x = X2 + 250;
                pos.y = Y2 - 350;
                Consent_Next.transform.localPosition = pos;
                Vector3 consentPos = new Vector3(60, 20, 0);
                consent_image.transform.localPosition = consentPos;
                //NotActiveObjects();
                con_toggle1.gameObject.SetActive(true);
                con_toggle2.gameObject.SetActive(true);
                con_toggle3.gameObject.SetActive(true);
                con_toggle4.gameObject.SetActive(true);
                con_toggle5.gameObject.SetActive(true);
                con_toggle6.gameObject.SetActive(true);

                con_manage1.gameObject.SetActive(true);
                con_manage2.gameObject.SetActive(true);
                con_manage3.gameObject.SetActive(true);
                con_manage4.gameObject.SetActive(true);
                con_manage5.gameObject.SetActive(true);
                con_manage6.gameObject.SetActive(true);
                main_number++;
                //image_number++;
            }
            else if (image_number == 1)
            {
                if (con_toggle1.isOn == true) //日本語へ
                {
                    Vector3 consentPos = new Vector3(0, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                    SetAllTogglesOff();
                    NotActiveObjects();
                    //con_toggle_nihon.gameObject.SetActive(true);
                    //con_manage7.gameObject.SetActive(true);
                    //image_number++;*/
                    Language = Selects.Japan;
                    selects = Selects.Japan;
                }
                else if (con_toggle2.isOn == true) //やさしい日本語へ
                {
                    Vector3 consentPos = new Vector3(0, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                    SetAllTogglesOff();
                    NotActiveObjects();
                    //con_toggle_yasashi.gameObject.SetActive(true);
                    //con_manage8.gameObject.SetActive(true);
                    //image_number++;*/
                    Language = Selects.Nihon;
                    selects = Selects.Nihon;
                }
                else if (con_toggle3.isOn == true) //英語へ
                {
                    Vector3 consentPos = new Vector3(0, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                    SetAllTogglesOff();
                    NotActiveObjects();
                    //con_toggle_eng.gameObject.SetActive(true);
                    //con_manage9.gameObject.SetActive(true);
                    //image_number++;*/
                    Language = Selects.English;
                    selects = Selects.English;
                }
                else if (con_toggle4.isOn == true) //中国語へ
                {
                    Vector3 consentPos = new Vector3(0, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                    SetAllTogglesOff();
                    NotActiveObjects();
                    //con_toggle_china.gameObject.SetActive(true);
                    //con_manage10.gameObject.SetActive(true);*/
                    //image_number++;
                    Language = Selects.China;
                    selects = Selects.China;
                }
                else if (con_toggle5.isOn == true) //ベトナム語へ
                {
                    Vector3 consentPos = new Vector3(0, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                    SetAllTogglesOff();
                    NotActiveObjects();
                    //image_number++;*/
                    Language = Selects.Beto;
                    selects = Selects.Beto;
                }
                else if (con_toggle6.isOn == true) //ミャンマー語へ
                {
                    Vector3 consentPos = new Vector3(0, 0, 0);
                    consent_image.transform.localPosition = consentPos;
                    SetAllTogglesOff();
                    NotActiveObjects();
                    //image_number++;*/
                    Language = Selects.Myan;
                    selects = Selects.Myan;
                }
            }
            else if (image_number == 2)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                language_number++;
            }
            else if (image_number == 3)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                Vector3 pos = new Vector3(X2, Y2, Z);
                pos.x = X2 - 180;
                pos.y = 0;
                consent_image.transform.localPosition = pos;
                understandManage.gameObject.SetActive(true);
                understandToggle.gameObject.SetActive(true);
                language_number++;
            }
            else if (image_number == 4)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                japan_agree.gameObject.SetActive(true);
                japan_disagree.gameObject.SetActive(true);
                japan_etc.gameObject.SetActive(true);
                con_maneJapan_agree.gameObject.SetActive(true);
                con_maneJapan_disagree.gameObject.SetActive(true);
                con_maneJapan_etc.gameObject.SetActive(true);
                language_number++;
            }
            else if (image_number == 5)
            {
                if (japan_agree.isOn == true) //同意した場合
                {
                    SetAllTogglesOff();
                    NotActiveObjects();
                    image_number = 7; //年齢確認画面へ
                    ageYes.gameObject.SetActive(true);
                    ageNo.gameObject.SetActive(true);
                    ageYes_mane.gameObject.SetActive(true);
                    ageNo_mane.gameObject.SetActive(true);
                    language_number = language_number + 3;
                    /*month.gameObject.SetActive(true);
                    date.gameObject.SetActive(true);
                    sign.gameObject.SetActive(true);
                    main_number = 3;
                    selects = Selects.Num;*/
                }
                else if (japan_disagree.isOn == true) // 同意しなかった場合
                {
                    SetAllTogglesOff();
                    NotActiveObjects();
                    language_number++;
                    backTitle_Button.gameObject.SetActive(true);
                }
                else if (japan_etc.isOn == true) //条件つき同意
                {
                    SetAllTogglesOff();
                    NotActiveObjects();
                    image_number = 6;
                    reason.gameObject.SetActive(true);
                    main_number = 2;
                    selects = Selects.Num;
                }
            }
            else if (image_number == 6) //同意しなかった場合
            {
                SetAllTogglesOff();
                NotActiveObjects();
            }
            else if (image_number == 7) //条件付き同意画面
            {
                SetAllTogglesOff();
                NotActiveObjects();
                ageYes.gameObject.SetActive(true);
                ageNo.gameObject.SetActive(true);
                ageYes_mane.gameObject.SetActive(true);
                ageNo_mane.gameObject.SetActive(true);
                language_number = language_number + 3;
                selects = Language;
                /*main_number++;
                month.gameObject.SetActive(true);
                date.gameObject.SetActive(true);
                sign.gameObject.SetActive(true);*/
            }
            else if (image_number == 8) // 年齢確認
            {
                if (ageYes.isOn == true) //18歳未満だった場合　（はい）
                {
                    SetAllTogglesOff();
                    NotActiveObjects();
                    consentYes.gameObject.SetActive(true);
                    //consentNo.gameObject.SetActive(true);
                    consentYes_mane.gameObject.SetActive(true);
                    //consentNo_mane.gameObject.SetActive(true);
                    language_number++;
                }
                else if(ageNo.isOn == true) //18歳以上だった場合　（いいえ）
                {
                    SetAllTogglesOff();
                    NotActiveObjects();
                    image_number = 9; //サイン画面へ
                    month.gameObject.SetActive(true);
                    date.gameObject.SetActive(true);
                    sign.gameObject.SetActive(true);
                    main_number = 3;
                    selects = Selects.Num;
                }
            }
            else if(image_number == 9)
            {
                if(consentYes.isOn == true)
                {
                    SetAllTogglesOff();
                    NotActiveObjects();
                    image_number = 9; //サイン画面へ
                    month.gameObject.SetActive(true);
                    date.gameObject.SetActive(true);
                    sign.gameObject.SetActive(true);
                    main_number = 3;
                    selects = Selects.Num;
                }
            }
            /*else if(image_number == 10)
            {
                SetAllTogglesOff();
                NotActiveObjects();
            }*/
            else if(image_number == 10) 
            {
                SetAllTogglesOff();
                NotActiveObjects();
                selects = Language;
                language_number = 5;
            }
            image_number++;
            ImageManager();
            if (reason.text != null)
            {
                Input_text[REASON] = reason.text;
            }
            if (month.text != null)
            {
                Input_text[MONTH] = month.text;
            }
            if (date.text != null)
            {
                Input_text[DATE] = date.text;
            }
            if (sign.text != null)
            {
                Input_text[SIGN] = sign.text;
            }
            /*for(int i =0; i<4; i++)
            {
                Debug.Log(Input_text[i]);
            }*/
        }
        if (this.mode == Mode.Data)
        {
            GetUserAnswer();
            image_number++;
            ImageManager();
            SetAllTogglesOff();
            NotActiveObjects();
            if (image_number == 1)
            {
                id.gameObject.SetActive(true);
                Vector3 pos = new Vector3(X2, Y2, Z);
                pos.x = X2 + 250;
                pos.y = Y2 - 350;
                Data_Next.transform.localPosition = pos;
            }
            else if (image_number == 2)
            {
                data_toggle_language1.gameObject.SetActive(true);
                data_toggle_language2.gameObject.SetActive(true);
                data_toggle_language3.gameObject.SetActive(true);
                data_toggle_language4.gameObject.SetActive(true);
                data_toggle_language5.gameObject.SetActive(true);
                data_toggle_language6.gameObject.SetActive(true);
                data_toggle_language7.gameObject.SetActive(true);
                data_toggle_language8.gameObject.SetActive(true);
                data_toggle_language9.gameObject.SetActive(true);
                data_toggle_language10.gameObject.SetActive(true);
                data_toggle_language11.gameObject.SetActive(true);
                data_toggle_language12.gameObject.SetActive(true);
                data_toggle_language13.gameObject.SetActive(true);
                data_toggle_language14.gameObject.SetActive(true);
                data_toggle_language15.gameObject.SetActive(true);
                data_toggle_language16.gameObject.SetActive(true);

                data_manage_Language1.gameObject.SetActive(true);
                data_manage_Language2.gameObject.SetActive(true);
                data_manage_Language3.gameObject.SetActive(true);
                data_manage_Language4.gameObject.SetActive(true);
                data_manage_Language5.gameObject.SetActive(true);
                data_manage_Language6.gameObject.SetActive(true);
                data_manage_Language7.gameObject.SetActive(true);
                data_manage_Language8.gameObject.SetActive(true);
                data_manage_Language9.gameObject.SetActive(true);
                data_manage_Language10.gameObject.SetActive(true);
                data_manage_Language11.gameObject.SetActive(true);
                data_manage_Language12.gameObject.SetActive(true);
                data_manage_Language13.gameObject.SetActive(true);
                data_manage_Language14.gameObject.SetActive(true);
                data_manage_Language15.gameObject.SetActive(true);
                data_manage_Language16.gameObject.SetActive(true);
            }
            else if (image_number == 3)
            {
                data_toggle1.gameObject.SetActive(true);
                data_toggle2.gameObject.SetActive(true);
                data_toggle3.gameObject.SetActive(true);
                data_toggle4.gameObject.SetActive(true);
                data_toggle5.gameObject.SetActive(true);

                data_manage1.gameObject.SetActive(true);
                data_manage2.gameObject.SetActive(true);
                data_manage3.gameObject.SetActive(true);
                data_manage4.gameObject.SetActive(true);
                data_manage5.gameObject.SetActive(true);
            }
            else if (image_number == 4)
            {
                data_toggle6.gameObject.SetActive(true);
                data_toggle7.gameObject.SetActive(true);
                data_toggle8.gameObject.SetActive(true);
                data_toggle9.gameObject.SetActive(true);
                data_toggle10.gameObject.SetActive(true);

                data_manage6.gameObject.SetActive(true);
                data_manage7.gameObject.SetActive(true);
                data_manage8.gameObject.SetActive(true);
                data_manage9.gameObject.SetActive(true);
                data_manage10.gameObject.SetActive(true);

            }
            else if (image_number == 5)
            {
                data_toggle11.gameObject.SetActive(true);
                data_toggle12.gameObject.SetActive(true);
                data_toggle13.gameObject.SetActive(true);
                data_toggle14.gameObject.SetActive(true);
                data_toggle15.gameObject.SetActive(true);
                data_toggle16.gameObject.SetActive(true);

                data_manage11.gameObject.SetActive(true);
                data_manage12.gameObject.SetActive(true);
                data_manage13.gameObject.SetActive(true);
                data_manage14.gameObject.SetActive(true);
                data_manage15.gameObject.SetActive(true);
                data_manage16.gameObject.SetActive(true);
            }
            else if (image_number == 6)
            {
                data_toggle17.gameObject.SetActive(true);
                data_toggle18.gameObject.SetActive(true);
                data_toggle19.gameObject.SetActive(true);
                data_toggle20.gameObject.SetActive(true);
                data_toggle21.gameObject.SetActive(true);

                data_manage17.gameObject.SetActive(true);
                data_manage18.gameObject.SetActive(true);
                data_manage19.gameObject.SetActive(true);
                data_manage20.gameObject.SetActive(true);
                data_manage21.gameObject.SetActive(true);
            }
            if (id.text != null)
            {
                Input_text[ID] = id.text;
            }
            
        }
        else if (this.mode == Mode.Question1)
        {
            if (back_flag == false)
            {
                //解答時間の記録とリセット
                ResponceTime = DateTime.Now - this.dt;
                dt = DateTime.Now;
                TimeConverter_Responce();

                responce = 0;
                //backTime = 0;
                TimeConverter_Back();
                //this.Question.backTime = backTime;
                toggle_previous = toggle_flag;
                BackCount = 0;

            }
            else
            {
                dt = DateTime.Now;
                backTime = DateTime.Now - this.back_dt;
                TimeConverter_Back();
                //this.Question.backTime = backTime.ToString();
                toggle_previous = toggle_flag;
                toggle_flag = toggle_present;
                ToggleManager();
                back_flag = false;
            }

            //各問題のログ取得
            //問題番号
            Question.number = Question_number_1[round, image_number];
            //問題文
            Question.question = "";  //追加
            //解答
            for (int i = 0; i < NUMBER_OF_ANSWER; i++)
            {
                Question.answerText.Add(Csv_answers_1[Question_number_1[round, image_number], csv_answers_num1[round, Question_number_1[round, image_number], i]]);
            }
            //ユーザーの解答
            GetUserAnswer();
            //ユーザーのフォント
            GetUserFont();

            //正誤判定
            if (Question.userAnswer == Csv_answers_1[csv_number, 0])
            {
                Question.judge = "〇";
            }
            else
            {
                Question.judge = "×";
            }
            image_number++;
            if (image_number == NUMBER_OF_QUESTION_1)
            {
                image_number = 0;
                round++;
            }
            csv_number = Question_number_1[round, image_number];
            ImageManager();
            SetAllTogglesOff();
            material1.gameObject.SetActive(false);
            material2.gameObject.SetActive(false);
            material3.gameObject.SetActive(false);
            material4.gameObject.SetActive(false);
            material_other.gameObject.SetActive(false);

            Question.backCount = BackCount;
            Question.type = "question";
            Question.userID = this.GetUserID(); 
            Question.userName = Input_text[SIGN];
            string jsonstr = JsonUtility.ToJson(this.Question);
            string.Join(",", Question.answerText.ToArray());
            Debug.Log(jsonstr);
            StartCoroutine(Post(SERVER_ADDRESS, jsonstr));
            //sw.WriteLine(jsonstr);

            Question1_Back.gameObject.SetActive(true);
            toggle_flag = 0;
            QuestionClear(); //追加
        }
        else if (this.mode == Mode.Question2)
        {
            if (back_flag == false)
            {
                //解答時間の記録とリセット
                ResponceTime = DateTime.Now - this.dt;
                dt = DateTime.Now;
                TimeConverter_Responce();
                responce = 0;
                //this.Question.responseTime = responceTime.ToString();
                //var backTime = 0;

                TimeConverter_Back();
                //this.Question.backTime = backTime.ToString();
                toggle_previous = toggle_flag;
                BackCount = 0;

            }
            else
            {
                dt = DateTime.Now;
                backTime = DateTime.Now - this.back_dt;
                TimeConverter_Back();
                //this.Question.backTime = backTime.ToString();
                toggle_previous = toggle_flag;
                toggle_flag = toggle_present;
                ToggleManager();
                back_flag = false;
            }
            //各問題のログ取得
            //問題番号
            Question.number = Question_number_2[round, image_number];
            //問題文
            Question.question = question_Text[csv_number].Japan;
            //解答
            for (int i = 0; i < NUMBER_OF_ANSWER; i++)
            {
                Question.answerText.Add(Csv_answers_2[Question_number_2[round, image_number], csv_answers_num2[round, Question_number_2[round, image_number], i]]);
                
            }
            //ユーザーの解答
            GetUserAnswer();
            //ユーザーのフォント
            GetUserFont();

            
            //正誤判定
            if(Question.userAnswer == Csv_answers_2[csv_number, 0]){
                Question.judge = "〇";
            }
            else
            {
                Question.judge = "×";
            }

            image_number++;
            if (image_number == NUMBER_OF_QUESTION_2)
            {
                image_number = 0;
                round++;
            }
            csv_number = Question_number_2[round, image_number];
            ImageManager();
            SetAllTogglesOff();
            material5.gameObject.SetActive(false);
            material6.gameObject.SetActive(false);
            material7.gameObject.SetActive(false);
            material8.gameObject.SetActive(false);


            Question.backCount = BackCount;
            Question.type = "question";
            Question.userID = this.GetUserID(); 
            Question.userName = Input_text[SIGN];
            string jsonstr = JsonUtility.ToJson(this.Question);
            Debug.Log(jsonstr);
            string.Join(",", Question.answerText.ToArray());
            //sw.WriteLine(jsonstr);

            StartCoroutine(Post(SERVER_ADDRESS, jsonstr));
            Question2_Back.gameObject.SetActive(true);
            toggle_flag = 0;

            QuestionClear(); //追加
        }
        else if (this.mode == Mode.Question3)
        {

            if (back_flag == false)
            {
                //解答時間の記録とリセット
                ResponceTime = DateTime.Now - this.dt;
                dt = DateTime.Now;
                TimeConverter_Responce();

                responce = 0;
                //backTime = 0;
                TimeConverter_Back();
                //this.Question.backTime = backTime;
                toggle_previous = toggle_flag;
                BackCount = 0;

            }
            else
            {
                dt = DateTime.Now;
                backTime = DateTime.Now - this.back_dt;
                TimeConverter_Back();
                //this.Question.backTime = backTime.ToString();
                toggle_previous = toggle_flag;
                toggle_flag = toggle_present;
                ToggleManager();
                back_flag = false;
            }
            //各問題のログ取得
            //問題番号
            Question.number = Question_number_3[round, image_number];
            //解答
            Question.question = Question3_Text[csv_number];
            //選択肢
            //Question.answerText.Add(""); //追加
            //ユーザーの解答
            GetUserAnswer();
            //ユーザーのフォント
            GetUserFont();

            //正誤判定
            if (Question.userAnswer == question3Answer[csv_number])
            {
                Question.judge = "〇";
            }
            else
            {
                Question.judge = "×";
            }

            image_number++;
            if (image_number == NUMBER_OF_QUESTION_3)
            {
                image_number = 0;
                round++;
            }
            csv_number = Question_number_3[round, image_number];

            ImageManager();
            SetAllTogglesOff();
            material9.gameObject.SetActive(false);
            material10.gameObject.SetActive(false);
            material11.gameObject.SetActive(false);

            Question.backCount = BackCount;
            Question.type = "question";
            Question.userID = this.GetUserID(); // 追加
            Question.userName = Input_text[SIGN];
            string jsonstr = JsonUtility.ToJson(this.Question);
            string.Join(",", Question.answerText.ToArray());
            Debug.Log(jsonstr);

            StartCoroutine(Post(SERVER_ADDRESS, jsonstr));
            back_flag = false;
            Question3_Back.gameObject.SetActive(true);
            toggle_flag = 0;
            QuestionClear();
        }
    }

    //メインバック関数
    public void Main_Back()
    {
        if (this.mode == Mode.Consent_Main)
        {
            if (image_number == 1)
            {
                SetAllTogglesOff();
                NotActiveObjects();

                Vector3 pos = new Vector3(X2, Y2, Z);
                pos.x = X2;
                pos.y = Y2;
                Consent_Next.transform.localPosition = pos;

                Vector3 consentPos = new Vector3(0, 0, 0);
                consent_image.transform.localPosition = consentPos;
                main_number = 0;
            }
            else if(image_number == 2)
            {
                SetAllTogglesOff();
                NotActiveObjects();

                con_toggle1.gameObject.SetActive(true);
                con_toggle2.gameObject.SetActive(true);
                con_toggle3.gameObject.SetActive(true);
                con_toggle4.gameObject.SetActive(true);
                con_toggle5.gameObject.SetActive(true);
                con_toggle6.gameObject.SetActive(true);

                con_manage1.gameObject.SetActive(true);
                con_manage2.gameObject.SetActive(true);
                con_manage3.gameObject.SetActive(true);
                con_manage4.gameObject.SetActive(true);
                con_manage5.gameObject.SetActive(true);
                con_manage6.gameObject.SetActive(true);

                Vector3 consentPos = new Vector3(60, 20, 0);
                consent_image.transform.localPosition = consentPos;
                main_number = 1; ;
                selects = Selects.Num;
            }

            else if(image_number == 3)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                language_number--;
            }
            else if(image_number == 4)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                language_number--;
            }
            else if(image_number == 5)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                understandManage.gameObject.SetActive(true);
                understandToggle.gameObject.SetActive(true);
                language_number--;
            }
            else if(image_number== 6 || image_number == 7 || image_number == 8)
            {
                image_number = 6;
                language_number = 3;
                SetAllTogglesOff();
                NotActiveObjects();
                japan_agree.gameObject.SetActive(true);
                japan_disagree.gameObject.SetActive(true);
                japan_etc.gameObject.SetActive(true);
                con_maneJapan_agree.gameObject.SetActive(true);
                con_maneJapan_disagree.gameObject.SetActive(true);
                con_maneJapan_etc.gameObject.SetActive(true);
                selects = Language;
            }
            else if(image_number == 9)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                ageYes.gameObject.SetActive(true);
                ageNo.gameObject.SetActive(true);
                ageYes_mane.gameObject.SetActive(true);
                ageNo_mane.gameObject.SetActive(true);
                language_number--;
            }
            else if(image_number == 10)
            {
                image_number = 9;
                SetAllTogglesOff();
                NotActiveObjects();
                ageYes.gameObject.SetActive(true);
                ageNo.gameObject.SetActive(true);
                ageYes_mane.gameObject.SetActive(true);
                ageNo_mane.gameObject.SetActive(true);
                language_number = 6;
                selects = Language;
            }
            else if(image_number == 11)
            {
                SetAllTogglesOff();
                NotActiveObjects();
                month.gameObject.SetActive(true);
                date.gameObject.SetActive(true);
                sign.gameObject.SetActive(true);
                main_number = 3;
                selects = Selects.Num;
            }
            image_number--;
            ImageManager();
            reason.text = Input_text[REASON];
            month.text = Input_text[MONTH];
            date.text = Input_text[DATE];
            sign.text = Input_text[SIGN];
            ImageManager();
        }

        else if (this.mode == Mode.Data)
        {
            image_number--;
            ImageManager();
            SetAllTogglesOff();
            NotActiveObjects();
            if(image_number == 0)
            {
                Vector3 dataPos = new Vector3(X2, Y2, Z2);
                dataPos.x = X2;
                dataPos.y = Y2;
            }
            else if (image_number == 1)
            {
                id.gameObject.SetActive(true);
            }
            else if (image_number == 2)
            {
                data_toggle_language1.gameObject.SetActive(true);
                data_toggle_language2.gameObject.SetActive(true);
                data_toggle_language3.gameObject.SetActive(true);
                data_toggle_language4.gameObject.SetActive(true);
                data_toggle_language5.gameObject.SetActive(true);
                data_toggle_language6.gameObject.SetActive(true);
                data_toggle_language7.gameObject.SetActive(true);
                data_toggle_language8.gameObject.SetActive(true);
                data_toggle_language9.gameObject.SetActive(true);
                data_toggle_language10.gameObject.SetActive(true);
                data_toggle_language11.gameObject.SetActive(true);
                data_toggle_language12.gameObject.SetActive(true);
                data_toggle_language13.gameObject.SetActive(true);
                data_toggle_language14.gameObject.SetActive(true);
                data_toggle_language15.gameObject.SetActive(true);
                data_toggle_language16.gameObject.SetActive(true);

                data_manage_Language1.gameObject.SetActive(true);
                data_manage_Language2.gameObject.SetActive(true);
                data_manage_Language3.gameObject.SetActive(true);
                data_manage_Language4.gameObject.SetActive(true);
                data_manage_Language5.gameObject.SetActive(true);
                data_manage_Language6.gameObject.SetActive(true);
                data_manage_Language7.gameObject.SetActive(true);
                data_manage_Language8.gameObject.SetActive(true);
                data_manage_Language9.gameObject.SetActive(true);
                data_manage_Language10.gameObject.SetActive(true);
                data_manage_Language11.gameObject.SetActive(true);
                data_manage_Language12.gameObject.SetActive(true);
                data_manage_Language13.gameObject.SetActive(true);
                data_manage_Language14.gameObject.SetActive(true);
                data_manage_Language15.gameObject.SetActive(true);
                data_manage_Language16.gameObject.SetActive(true);
            }
            else if (image_number == 3)
            {
                data_toggle1.gameObject.SetActive(true);
                data_toggle2.gameObject.SetActive(true);
                data_toggle3.gameObject.SetActive(true);
                data_toggle4.gameObject.SetActive(true);
                data_toggle5.gameObject.SetActive(true);

                data_manage1.gameObject.SetActive(true);
                data_manage2.gameObject.SetActive(true);
                data_manage3.gameObject.SetActive(true);
                data_manage4.gameObject.SetActive(true);
                data_manage5.gameObject.SetActive(true);
            }
            else if (image_number == 4)
            {
                data_toggle6.gameObject.SetActive(true);
                data_toggle7.gameObject.SetActive(true);
                data_toggle8.gameObject.SetActive(true);
                data_toggle9.gameObject.SetActive(true);
                data_toggle10.gameObject.SetActive(true);

                data_manage6.gameObject.SetActive(true);
                data_manage7.gameObject.SetActive(true);
                data_manage8.gameObject.SetActive(true);
                data_manage9.gameObject.SetActive(true);
                data_manage10.gameObject.SetActive(true);
            }
            else if (image_number == 5)
            {
                data_toggle11.gameObject.SetActive(true);
                data_toggle12.gameObject.SetActive(true);
                data_toggle13.gameObject.SetActive(true);
                data_toggle14.gameObject.SetActive(true);
                data_toggle15.gameObject.SetActive(true);
                data_toggle16.gameObject.SetActive(true);

                data_manage11.gameObject.SetActive(true);
                data_manage12.gameObject.SetActive(true);
                data_manage13.gameObject.SetActive(true);
                data_manage14.gameObject.SetActive(true);
                data_manage15.gameObject.SetActive(true);
                data_manage16.gameObject.SetActive(true);
            }
            else if (image_number == 6)
            {
                data_toggle17.gameObject.SetActive(true);
                data_toggle18.gameObject.SetActive(true);
                data_toggle19.gameObject.SetActive(true);
                data_toggle20.gameObject.SetActive(true);
                data_toggle21.gameObject.SetActive(true);

                data_manage17.gameObject.SetActive(true);
                data_manage18.gameObject.SetActive(true);
                data_manage19.gameObject.SetActive(true);
                data_manage20.gameObject.SetActive(true);
                data_manage21.gameObject.SetActive(true);
            }
            id.text = Input_text[ID];
            language.text = Input_text[LANGUAGE];
        }
        else if (this.mode == Mode.Question1)
        {
            /*
            //問題を戻る前に問題を見ていた時間を記録
             */
            ResponceTime = DateTime.Now - dt; 
            var Num = ResponceTime;
            float time;
            int seconds, milliseconds;
            seconds = Num.Seconds;
            milliseconds = Num.Milliseconds;
            time = (float)seconds + ((float)milliseconds / 1000);
            responce = time;
            /*
             */
            back_dt = DateTime.Now;
            BackCount++;
            toggle_present = toggle_flag;
            image_number--;
            if (image_number < 0)
            {
                image_number = NUMBER_OF_QUESTION_1-1;
                round--;
            }
            csv_number = Question_number_1[round, image_number];
            ImageManager();
            Question1_Back.gameObject.SetActive(false);
            SetAllTogglesOff();
            material1.gameObject.SetActive(false);
            material2.gameObject.SetActive(false);
            material3.gameObject.SetActive(false);
            material4.gameObject.SetActive(false);
            material_other.gameObject.SetActive(false);
            toggle_flag = toggle_previous;
            ToggleManager();
            back_flag = true;
        }
        else if(this.mode == Mode.Question2)
        {
            /*
             //問題を戻る前に問題を見ていた時間を記録
              */
            ResponceTime = DateTime.Now - dt;
            var Num = ResponceTime;
            float time;
            int seconds, milliseconds;
            seconds = Num.Seconds;
            milliseconds = Num.Milliseconds;
            time = (float)seconds + ((float)milliseconds / 1000);
            responce = time;
            /*
             */

            back_dt = DateTime.Now;
            BackCount++;
            toggle_present = toggle_flag;
            image_number--;
            if (image_number < 0)
            {
                image_number = NUMBER_OF_QUESTION_2 - 1;
                round--;
            }
            csv_number = Question_number_2[round, image_number];
            ImageManager();
            Question2_Back.gameObject.SetActive(false);
            SetAllTogglesOff();
            material5.gameObject.SetActive(false);
            material6.gameObject.SetActive(false);
            material7.gameObject.SetActive(false);
            material8.gameObject.SetActive(false);
            toggle_flag = toggle_previous;
            ToggleManager();
            back_flag = true;
        }
        else if(this.mode == Mode.Question3)
        {
            /*
            //問題を戻る前に問題を見ていた時間を記録
             */
            ResponceTime = DateTime.Now - dt;
            var Num = ResponceTime;
            float time;
            int seconds, milliseconds;
            seconds = Num.Seconds;
            milliseconds = Num.Milliseconds;
            time = (float)seconds + ((float)milliseconds / 1000);
            responce = time;
            /*
             */
            back_dt = DateTime.Now;
            BackCount++;
            toggle_present = toggle_flag;
            //flag = false;
            image_number--;
            if (image_number < 0)
            {
                image_number = NUMBER_OF_QUESTION_3 - 1;
                round--;
            }
            csv_number = Question_number_3[round, image_number];
            ImageManager();
            SetAllTogglesOff();
            material9.gameObject.SetActive(false);
            material10.gameObject.SetActive(false);
            material11.gameObject.SetActive(false);
            toggle_flag = toggle_previous;
            ToggleManager();
            back_flag = true;
            Question3_Back.gameObject.SetActive(true);
        }
    }

    //メインチェンジ関数
    public void Main_Change()
    {
        if (this.mode == Mode.Consent_Main)
        {
            Title_Change_Button.gameObject.SetActive(true);
            image_number = 0;

            //各入力結果のログを取得
            Consent.userChoiceConsent = Language.ToString();
            Consent.userDetail = Input_text[REASON];
            Consent.month = Input_text[MONTH];
            Consent.date = Input_text[DATE];
            Consent.userName = Input_text[SIGN];

            Consent.type = "consent";
            Consent.userID = this.GetUserID(); // 追加
            Consent.userName = Input_text[SIGN];
            string jsonstr = JsonUtility.ToJson(this.Consent);
            //Debug.Log(jsonstr);

            //sw.WriteLine(jsonstr);
            StartCoroutine(Post(SERVER_ADDRESS, jsonstr));
            Resources.UnloadAsset(consent_image.sprite);
            consent_image.sprite = null;
            this.mode = Mode.Data;
        }
        else if (this.mode == Mode.Data)
        {
            GetUserAnswer();
            Title_Change_Button.gameObject.SetActive(true);
            image_number = 0;

            DataPaper.userId = Input_text[ID];
            DataPaper.userLanguage = Input_text[LANGUAGE];
            for(int i =0; i < 6; i++)
            {
                if (userChoice_data[0, i] != null && userChoice_data[0, i] != "")
                    {
                    DataPaper.userChoiceStudy = userChoice_data[0, i];
                }
                if (userChoice_data[1, i] != null && userChoice_data[1, i] != "")
                {
                    DataPaper.userChoiceHowLearn.Add(userChoice_data[1, i]);
                }
                if (userChoice_data[2, i] != null && userChoice_data[2, i] != "")
                {
                    DataPaper.userChoiceJLPT = userChoice_data[2, i];
                }
                if (userChoice_data[3, i] != null && userChoice_data[3, i] != "")
                {
                    DataPaper.age = userChoice_data[3, i];
                }
            }

            DataPaper.type = "dataPaper";
            DataPaper.userID = this.GetUserID(); // 追加
            DataPaper.userName = Input_text[SIGN];
            string jsonstr = JsonUtility.ToJson(this.DataPaper);
            Debug.Log(jsonstr);

            //sw.WriteLine(jsonstr);

            StartCoroutine(Post(SERVER_ADDRESS, jsonstr));

            title_number++;
            Resources.UnloadAsset(data_image.sprite);
            data_image.sprite = null;

            QuestionClear();
            this.mode = Mode.Title;
        }
        else if (this.mode == Mode.Question1)
        {
            if (back_flag == false)
            {
                //解答時間の記録とリセット
                ResponceTime = DateTime.Now - this.dt;
                dt = DateTime.Now;
                /*this.Question.responseTime = responceTime.ToString();
                var backTime = 0;
                this.Question.backTime = backTime.ToString();*/
                TimeConverter_Responce();
                TimeConverter_Back();

            }
            else
            {
                dt = DateTime.Now;
                backTime = DateTime.Now - this.back_dt;
                TimeConverter_Back();
                //this.Question.backTime = backTime.ToString();
            }
            //各問題のログ取得
            //問題番号
            Question.number = Question_number_1[round, image_number];
            //問題
            Question.question = "";  //追加
            //解答
            for (int i = 0; i < NUMBER_OF_ANSWER; i++)
            {
                Question.answerText.Add(Csv_answers_1[Question_number_1[round, image_number], csv_answers_num1[round, Question_number_1[round, image_number], i]]);
            }
            //ユーザーの解答
            GetUserAnswer();
            //ユーザーのフォント
            GetUserFont();

            //正誤判定
            if (Question.userAnswer == Csv_answers_1[csv_number, 0])
            {
                Question.judge = "〇";
            }
            else
            {
                Question.judge = "×";
            }

            Question.backCount = BackCount;
            Question.type = "question";
            Question.userID = this.GetUserID(); 
            Question.userName = Input_text[SIGN];
            string jsonstr = JsonUtility.ToJson(this.Question);
            string.Join(",", Question.answerText.ToArray());
            Debug.Log(jsonstr);
            StartCoroutine(Post(SERVER_ADDRESS, jsonstr));

            title_number++;
            QuestionClear(); //追加

            Title_Change_Button.gameObject.SetActive(true);
            this.mode = Mode.Title;
        }
        else if (this.mode == Mode.Question2)
        {
            if (back_flag == false)
            {
                //解答時間の記録とリセット
                ResponceTime = DateTime.Now - this.dt;
                dt = DateTime.Now;
                /*this.Question.responseTime = responceTime.ToString();
                var backTime = 0;
                this.Question.backTime = backTime.ToString();*/
                TimeConverter_Responce();
                TimeConverter_Back();

            }
            else
            {
                dt = DateTime.Now;
                backTime = DateTime.Now - this.back_dt;
                //this.Question.backTime = backTime.ToString();
                TimeConverter_Back();
            }
            //各問題のログ取得
            //問題番号
            Question.number = Question_number_2[round, image_number];
            //問題文
            Question.question = question_Text[csv_number].Japan; //追加
            //解答
            for (int i = 0; i < NUMBER_OF_ANSWER; i++)
            {
                Question.answerText.Add(Csv_answers_2[Question_number_2[round, image_number], csv_answers_num2[round, Question_number_2[round, image_number], i]]);
            }
            //ユーザーの解答
            GetUserAnswer();
            //ユーザーのフォント
            GetUserFont();

            //正誤判定
            if (Question.userAnswer == Csv_answers_2[csv_number, 0])
            {
                Question.judge = "〇";
            }
            else
            {
                Question.judge = "×";
            }


            Question.backCount = BackCount;
            Question.type = "question";
            Question.userID = this.GetUserID(); 
            Question.userName = Input_text[SIGN];
            string jsonstr = JsonUtility.ToJson(this.Question);
            string.Join(",", Question.answerText.ToArray());
            Debug.Log(jsonstr);
            StartCoroutine(Post(SERVER_ADDRESS, jsonstr));
            title_number++;

            QuestionClear(); //追加

            Title_Change_Button.gameObject.SetActive(true);
            this.mode = Mode.Title;
        }
    }


    //　タイトルチェンジボタン関数
    public void OnClick_Title_Change_Button()
    {
        image_number = 0;
        csv_number = 0;
        toggle_flag = 0;
        toggle_present = 0;
        toggle_previous = 0;
        if (title_number == 0)
        {
            this.mode = Mode.Consent_Main;
        }
        else if(title_number == 1)
        {
            title_number++;
            TitleManager();
        }
        else if (title_number == 2)
        {
            this.mode = Mode.Question1;
        }
        else if (title_number == 3)
        {
            title_number++;
            TitleManager();
        }
        else if (title_number == 4)
        {
            this.mode = Mode.Question2;
        }
        else if (title_number == 5)
        {
            title_number++;
            TitleManager();
        }
        else if (title_number == 6)
        {
            this.mode = Mode.Question3;
        }
    }

    //　タイトルに戻る
    public void backTitle()
    {
        image_number = 0;
        main_number = 0;
        csv_number = 0;
        toggle_flag = 0;
        toggle_present = 0;
        toggle_previous = 0;
        title_number = 0;
        language_number = 0;
        Vector3 consentPos = new Vector3(0, 0, 0);
        consent_image.transform.localPosition = consentPos;
        selects = Selects.Num;
        mode = Mode.Title;
    }
    private void QuestionClear()
    {
        //問題のログ
        Question.number = default;
        Question.question = default;
        Question.answerText = new List<string>();
        Question.userAnswer = default;
        Question.font = default;
        Question.judge = default;
        //Question.responseTime = default;
        //Question.backCount = default;
        //Question.backTime = default;
    }

    // 終了ボタンを押した時のボタン関数
    public void Onclick_End_Button()
    {
        if (back_flag == false)
        {
            //解答時間の記録とリセット
            ResponceTime = DateTime.Now - dt;
            dt = DateTime.Now;
            TimeConverter_Responce();
            responce = 0;
            TimeConverter_Back();
            /*this.Question.responseTime = responceTime.ToString();
            var backTime = 0;
            this.Question.backTime = backTime.ToString();*/
            BackCount = 0;
        }
        else
        {
            dt = DateTime.Now;
            backTime = DateTime.Now - this.back_dt;
            TimeConverter_Back();
            //this.Question.backTime = backTime.ToString();
            back_flag = false;
        }
        //各問題のログ取得
        //問題番号
        Question.number = Question_number_3[round, image_number];
        //問題文
        Question.question = Question3_Text[csv_number];
        //選択肢
       // Question.answerText.Add("");
        //ユーザーの解答
        GetUserAnswer();
        //ユーザーのフォント
        GetUserFont();

        //正誤判定
        if (Question.userAnswer == question3Answer[csv_number])
        {
            Question.judge = "〇";
        }
        else
        {
            Question.judge = "×";
        }

        Question.backCount = BackCount;
        Question.type = "question";
        Question.userID = this.GetUserID(); // 追加
        Question.userName = Input_text[SIGN];
        string jsonstr = JsonUtility.ToJson(this.Question);
        string.Join(",", Question.answerText.ToArray());
        Debug.Log(jsonstr);
        StartCoroutine(Post(SERVER_ADDRESS, jsonstr));

        title_number++;
        QuestionClear();
        this.mode = Mode.Title;
        Vector3 titlePos = new Vector3(0, -600, 0);
        Data_Next.transform.localPosition = titlePos;
        title.gameObject.SetActive(true);
        backTitle_Button2.gameObject.SetActive(true);
        TitleManager();
    }

    IEnumerator Post(string url, string jsonstr)
    {
        Debug.Log("Post to URL: " + url);
        Debug.Log("Send Data" + jsonstr);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Status Code: " + request.responseCode);

        yield return request.SendWebRequest();
    }
}

public class Question1_Item
{
    public int Number_Question1 { get; }
    public int Random_Question1 { get; }

    public Question1_Item(int num_question1, int random_question1)
    {
        Number_Question1 = num_question1;
        Random_Question1 = random_question1;

    }
}
public class Item
{
    public int Number { get; }
    public string Value { get; }

    public Item(int num,string value)
    {
        Number = num;
        Value = value;

    }
}

public class Item_Question2_Text
{
    public string Number_Question2_Text { get; }
    public string Value_Question2_Text { get; }

    public Item_Question2_Text(string num_question2_text, string value_question2_text)
    {
        Number_Question2_Text = num_question2_text;
        Value_Question2_Text = value_question2_text;

    }
}

public class Item_Question2_Answer
{
    public int Number_Question2_Answer { get; }
    public string Value_Question2_Answer { get; }

    public Item_Question2_Answer(int num_question2_answer, string value_question2_answer)
    {
        Number_Question2_Answer = num_question2_answer;
        Value_Question2_Answer = value_question2_answer;

    }
}

public class Item_Question3_Text
{
    public int Number_Question3_Text { get; }
    public string Value_Question3_Text { get; }
    public string Answer_question3 { get; }

    public Item_Question3_Text(int num_question3_text, string value_question3_text,string answer_question3)
    {
        Number_Question3_Text = num_question3_text;
        Value_Question3_Text = value_question3_text;
        Answer_question3 = answer_question3;

    }
}

public class Question_Text
{
    public string Japan { get; }
    public string English { get; }

    public Question_Text(string english, string japan)
    {
        Japan = japan;
        English = english;
    }
}

//シャッフルクラス
public static class ShuffleExtensions
{
    /// <summary>
    /// 指定された要素の配列をシャッフルする
    /// </summary>
    public static void Shuffle<T>(this IList<T> array)
    {
        for (var i = array.Count - 1; i > 0; --i)
        {
            // 0以上i以下のランダムな整数を取得
            // Random.Rangeの最大値は第２引数未満なので、+1することに注意
            var j = Random.Range(0, i + 1);

            // i番目とj番目の要素を交換する
            var tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }
    }
}

public static class NumberShuffle
{
    /// <summary>
    /// 問題番号のシャッフル
    /// </summary>
    public static void Number_Shuffle<T>(this IList<T> array)
    {
        for (var i = 0; i < array.Count; i = i + array.Count/3)
        {
            for (var j = i + (array.Count/3 - 1); j > i; j--)
            {
                // 0以上i以下のランダムな整数を取得
                // Random.Rangeの最大値は第２引数未満なので、+1することに注意
                var z = Random.Range(i, j);

                // i番目とj番目の要素を交換する
                var tmp = array[j];
                array[j] = array[z];
                array[z] = tmp;
            }
        }
    }
}

public static class FontShuffle
{
    /// <summary>
    /// フォント配列のシャッフル
    /// </summary>
    public static void Font_Shuffle<T>(this IList<T> array)
    {
        for (var i = 0; i < array.Count; i = i+3)
        {
            for (var j = i+2; j > i; j--)
            {
                // 0以上i以下のランダムな整数を取得
                // Random.Rangeの最大値は第２引数未満なので、+1することに注意
                var z = Random.Range(i, j);

                // i番目とj番目の要素を交換する
                var tmp = array[j];
                array[j] = array[z];
                array[z] = tmp;
            }
        }
    }
}

public static class AnswerShuffle
{
    /// <summary>
    /// 問題番号のシャッフル
    /// </summary>
    public static void Answer_Shuffle<T>(this IList<T> array)
    {
        for (var i = 0; i < array.Count; i = i + 4)
        {
            for (var j = i + 3; j > i; j--)
            {
                // 0以上i以下のランダムな整数を取得
                // Random.Rangeの最大値は第２引数未満なので、+1することに注意
                var z = Random.Range(i, j);

                // i番目とj番目の要素を交換する
                var tmp = array[j];
                array[j] = array[z];
                array[z] = tmp;
            }
        }
    }
}