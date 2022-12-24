using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class newUI : MonoBehaviour
{
    public float UINumber;
    public float Timer = 0;
    public float ySpeed = 200;
    // Start is called before the first frame update
    void Start()
    {
        Timer = 0;
        this.GetComponent<Text>().text = UINumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer>= 0.7f)
        {
            GameObject.Destroy(this.gameObject);
        }
        ySpeed -= 450f * Time.deltaTime;

        this.GetComponent<RectTransform>().localPosition += new Vector3(200,ySpeed,0) * Time.deltaTime;
        
    }
}
