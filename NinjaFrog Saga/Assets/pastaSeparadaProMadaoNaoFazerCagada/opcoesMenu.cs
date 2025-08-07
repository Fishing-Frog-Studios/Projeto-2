using UnityEngine;
using System.IO;

public class opcoesMenu : MonoBehaviour
{

    public GameObject isfrioNe;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isfrioNe != null)
        {

            isfrioNe.SetActive(false);
        }
        
    }

    public void mostrar()
    {
        if (isfrioNe != null)
        {

            isfrioNe.SetActive(true);
        }

    }


    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            isfrioNe.SetActive(false);
        }

        

        
    }
}
