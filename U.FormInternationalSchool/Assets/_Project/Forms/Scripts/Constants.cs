using UnityEngine;

public class Constants : MonoBehaviour
{
    public const string IMAGE_TITLE = "gameTitleImageUrl";
    public const string IMAGE_BACK = "backgroundUrl";
    public const string MUSIC_BACK = "backgroundMusicUrl";
    public const string AUDIO_PT = "questionStatementPortugueseAudioUrl";
    public const string AUDIO_EN = "questionStatementEnglishAudioUrl";

    public const string GAME_SO = "GAME_SO";
    public const string GAME_EDIT = "GAME_EDIT";
    public const string IS_EDIT = "IS_EDIT";
    public const string GAME_SETTINGS = "GAME_SETTINGS";

 //    public const string URL_DATABASE = "https://gamehubapi.internationalschool.global/";
       public const string URL_DATABASE = "https://school.gamehub.api.oke.luby.me/";
    //public const string URL_DATABASE = "https://dev-is-portal-atividades.internationalschool.global/";
    public const string URL_UPLOAD_MEDIA = URL_DATABASE + "file-upload";

    public const string ERROR_IMAGE_URL =
        "https://edtechprojetos.blob.core.windows.net/arquivos/e0b7d321-fcaf-44bf-ac33-bb7fc8679e72_name.Vector.png";

    public const string ERROR_IMAGE_URL_SQUARE = "https://edtechprojetos.blob.core.windows.net/arquivos/217cbcc2-c2b3-41a3-9b77-f44eb8f077ba_name.pair_left.png";
}