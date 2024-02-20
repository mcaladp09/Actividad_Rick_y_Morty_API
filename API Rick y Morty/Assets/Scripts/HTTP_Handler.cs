using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HTTP_Handler : MonoBehaviour
{
    private string fakeApiUrl = "https://my-json-server.typicode.com/mcaladp09/Actividad_API_Rick_y_Morty";
    private string RickAndMoryApiUrl = "https://rickandmortyapi.com/api";
    private int imagesDownloadsCounter;//

    public List<RawImage> cardsImages = new List<RawImage>();//name
    public List<TextMeshProUGUI> cardsNames = new List<TextMeshProUGUI>();

    Coroutine sendRequest_GetCharacters;

    public void SendRequest(int userID)
    {

        imagesDownloadsCounter = 0;
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
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);
                foreach (int cardID in user.deck)
                {
                    StartCoroutine(GetCharacters(cardID, imagesDownloadsCounter));
                    imagesDownloadsCounter++;
                }
            }
        }
    }

    IEnumerator GetCharacters(int cardId, int deckIndex)
    {
        UnityWebRequest request = UnityWebRequest.Get(RickAndMoryApiUrl + "/character/" + cardId);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.responseCode);//

            if (request.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                cardsNames[deckIndex].text = character.name;
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
            cardsImages[deckIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
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

