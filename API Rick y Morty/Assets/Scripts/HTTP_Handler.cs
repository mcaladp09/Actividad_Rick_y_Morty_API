using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class HTTP_Handler : MonoBehaviour
{
    private string fakeApiUrl = "https://my-json-server.typicode.com/mcaladp09/Actividad_Rick_y_Morty_API";
    private string RickyMortyAPIURL = "https://rickandmortyapi.com/api";
    private int countImages;

    public List<TextMeshProUGUI> cardNamesList = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> cardIDsList = new List<TextMeshProUGUI>();
    public List<RawImage> cardImagesLists = new List<RawImage>();

    Coroutine sendRequest_GetCharacters;

    public void SendRequest(int userID)
    {

        sendRequest_GetCharacters = StartCoroutine(GetUserData(userID));

    }

    IEnumerator GetUserData(int uid)
    {
        UnityWebRequest request = UnityWebRequest.Get(fakeApiUrl + "/users/" + uid);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                countImages = 0;
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);
                foreach (int cardID in user.deck)
                {
                    StartCoroutine(GetCharacters(cardID, countImages));
                    countImages++;
                }
            }
        }
    }

    IEnumerator GetCharacters(int cardId, int deckIndex)
    {
        UnityWebRequest request = UnityWebRequest.Get(RickyMortyAPIURL + "/character/" + cardId);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.responseCode);

            if (request.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                cardNamesList[deckIndex].text = character.name;
                cardIDsList[deckIndex].text = character.id.ToString();
                StartCoroutine(DownloadImage(character.image, deckIndex));
            }
        }
    }

    IEnumerator DownloadImage(string url, int deckIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            cardImagesLists[deckIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}

[System.Serializable]
public class UserData
{
    public int id;
    public string username;
    public int[] deck;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string image;
}
