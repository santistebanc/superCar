using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    public Text timerLabel;
    public Text lapsLabel;
    public Text lapscounter;

    public Text entertostart;
    public Text finallaptext;

    public GameObject place;
    public AudioSource[] beeps;

    public AudioClip backmusic;
    public AudioClip racestart;
    public AudioClip winrace;
    public AudioClip loserace;
    public AudioClip finallap;

    public RawImage semaphore;

    public Material sem0;
    public Material sem1;
    public Material sem2;
    public Material sem3;

    public int wincount = 0;

    public Font myfont;

    public CanvasRenderer cr;

    public int laps = 1;
    public int currentlap = 1;

    private float time = 0;

    public bool startcount = false;

    public float count = 0;

    public bool startrace = false;

    private bool finished = false;

    private int audioplayed = 0;

    float minutes;
    float seconds;
    float fraction;

    void Start()
    {
        beeps = place.GetComponents<AudioSource>();
        beeps[4].PlayOneShot(racestart);
    }

    void Update()
    {
        if (!startrace && Input.GetAxis("Submit") == 1)
        {
            startcount = true;
            
        }
        if (startcount)
        {
            if (count < 1f)
            {
                semaphore.enabled = true;
                semaphore.material = sem0;
            }
            else if (count < 2f)
            {
                if (audioplayed == 0)
                {
                    beeps[0].Play();
                    audioplayed++;
                }
                semaphore.material = sem1;
            }
            else if (count < 3f)
            {
                if (audioplayed == 1)
                {
                    beeps[1].Play();
                    audioplayed++;
                }
                semaphore.material = sem2;
            }
            else if (count < 4f)
            {
                if (audioplayed == 2)
                {
                    beeps[2].Play();
                    audioplayed++;
                }
                semaphore.material = sem3;
                startrace = true;
            }
            else if (count < 4.5f)
            {
                if (audioplayed == 3)
                {
                    beeps[3].loop = true;
                    beeps[3].PlayOneShot(backmusic);
                    audioplayed++;
                }
            }
            else if (count < 5.5f)
            {
                startcount = false;
                audioplayed = 0;
                count = 0;
                semaphore.enabled = false;
            }
        }

        if (Input.GetAxis("Restart") == 1){
            for (int i = 0; i < beeps.Length; i++)
            {
                beeps[i].Stop();
            }
                startrace = false;
            finished = false;
            wincount = 0;
            currentlap = 1;
            time = 0;

            int childs = cr.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(cr.transform.GetChild(i).gameObject);
            }

            BroadcastMessage("resetIt");
        }

        if (startcount)
        {
            count += Time.deltaTime;
        }


        if (startrace)
        {
            time += Time.deltaTime;
        }

        minutes = Mathf.Floor(time / 60);
        seconds = Mathf.Floor(time) % 60;
        fraction = Mathf.Floor(time * 100) % 100;

        if (!finished)
        {
            timerLabel.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);
            lapscounter.text = "Lap " + currentlap + " / " + laps;
        }

        if (!finished && !startrace && !startcount)
        {
            entertostart.enabled = true;
        }
        else
        {
            entertostart.enabled = false;
        }

    }
    public void addlap(Transform who,int lap,Color color)
    {
        if (lap == laps + 1)
        {
            crossGoal(who, color);
        }

        if (who.name == "Car")
        {
            if (lap == laps + 1)
            {
                if (wincount < 4)
                {
                    beeps[3].Stop();
                    beeps[4].PlayOneShot(winrace);
                }
                else
                {
                    beeps[3].Stop();
                    beeps[4].PlayOneShot(loserace);
                }
                finished = true;
            }
            else if (lap < laps + 1)
            {
                currentlap = lap;
                lapsLabel.text = timerLabel.text + "\n" + lapsLabel.text;
                if (lap == laps)
                {
                    //final lap
                    pausemusicbriefly(2.5f);
                    beeps[4].PlayOneShot(finallap);
                    finallaptext.text = "Final Lap";
                    finallaptext.enabled = true;
                    Invoke("removemes", 2);
                }
            }
        }
        
    }

    public void pausemusicbriefly(float time)
    {
        beeps[3].Pause();
        Invoke("unpausemusic", time);
    }

    public void removemes()
    {
        finallaptext.enabled = false;
    }

    public void unpausemusic()
    {
        beeps[3].UnPause();
    }

    public void crossGoal(Transform who, Color color)
    {
        GameObject newGO = new GameObject("myTextGO");
        newGO.transform.SetParent(cr.transform);

        Text myText = newGO.AddComponent<Text>();
        Vector3 pos = new Vector3(35, 145 - 20 * wincount, 0);
        myText.transform.localPosition = pos;
        wincount++;
        myText.font = myfont;
        myText.fontStyle = FontStyle.Bold;
        myText.alignment = TextAnchor.UpperRight;
        myText.color = color;
        myText.text = (wincount)+ ".   " + string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);;
    }

}