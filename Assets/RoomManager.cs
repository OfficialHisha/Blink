using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    bool playing = false;

    [HideInInspector]
    public int players;

    public TextMeshProUGUI topText;
    public TextMeshProUGUI countDownText;

    public GameObject startCollider;

    int minutes, seconds = -10, frames;

    public GameObject scoreboardCanvas;
    public GameObject scoreboardContent;
    public GameObject scoreboardEntryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.instance.Send(new LoadedPacket());
    }

    // Update is called once per frame
    void Update()
    {
        if (!playing)
            return;

        if (seconds >= 0)
            frames += 6;
            

        frames = Mathf.Clamp(frames, 0, 999);

        countDownText.text = $"{minutes.ToString("00")}:{seconds.ToString("00")}:{frames.ToString("000")}";
    }

    public void AllLoaded()
    {
        playing = true;
        StartCoroutine(Counter());
    }

    public void AllFinished()
    {
        playing = false;

        scoreboardCanvas.SetActive(true);

        int place = 1;
        foreach (KeyValuePair<string, TimeStamp> player in NetworkManager.instance.scores)
        {
            GameObject obj = Instantiate(scoreboardEntryPrefab, scoreboardContent.transform);
            obj.GetComponent<TextMeshProUGUI>().text = $"{place++}. {player.Key} - {player.Value.ToString()}";
        }
    }

    IEnumerator Counter()
    {
        while (playing)
        {
            yield return new WaitForSeconds(1);
            seconds++;
            frames = 0;

            if (seconds == 0)
                Launch();

            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }
        }
    }

    void Launch()
    {
        Destroy(startCollider);
        topText.text = "Go!";

    }

    public TimeStamp GetTimeStamp()
    {
        return new TimeStamp(minutes, seconds, frames);
    }
}