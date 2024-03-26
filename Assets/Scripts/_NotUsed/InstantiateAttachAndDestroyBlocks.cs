using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GoogleVR;
//using GoogleVR.Beta;

public class InstantiateAttachAndDestroyBlocks : MonoBehaviour
{
    //informacoes do gameobject que recebeu o hit e raio que será emitido
    RaycastHit hit;

    Ray ray;

    //os cubos que vamos pegar   
    GameObject selectedCube;
    public GameObject forwardDispenser;
    public GameObject backDispenser;
    public GameObject leftDispenser;
    public GameObject rightDispenser;
    public GameObject beginDispenser;
    public GameObject endDispenser;
    //mão do player que vai ser pai do cubo, que vai segurar o cubo
    public GameObject playerHands;
    //só para controlar se há algum cubo nas mãos do player
    bool inHands = false;
    //placas de programacao que o cubo pode ser anexado
    //podia ser uma list
    public GameObject codingCell1;
    public GameObject codingCell2;
    public GameObject codingCell3;
    public GameObject codingCell4;
    public GameObject codingCell5;
    public GameObject codingCell6;
    public GameObject codingCell7;
    public GameObject codingCell8;

    //controla o som
    private AudioSource source;
    public AudioClip selectingCube;
    public AudioClip releasingCube;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //fica verificando se o fire1 é disparado

        if(Input.GetButtonDown("Fire1"))
        {
            Debug.Log("%%% Fire1 pressionado");

            if(inHands) // anexa o cubo a uma placa de programação ou destrói o cubo
            {

                //verifica para onde o usuário está olhando quando clica e então retorna o nome do gameobject clicado
                //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //if (Physics.Raycast(ray, out hit))
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
                {
                    Debug.Log("#### Raio hit e inHands: " + hit.transform.name + " --- " + inHands);

                    if(selectedCube && hit.transform.name.Contains("CodingCell"))
                    {

                        Debug.Log("@@ CoddingCell selecionada: " + hit.transform.name);

                        switch(hit.transform.name)
                        {
                            case "CodingCell1":
                                if(codingCell1.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell1.transform);
                                break;

                            case "CodingCell2":
                                if(codingCell2.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell2.transform);
                                break;

                            case "CodingCell3":
                                if(codingCell3.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell3.transform);
                                break;

                            case "CodingCell4":
                                if(codingCell4.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell4.transform);
                                break;

                            case "CodingCell5":
                                if(codingCell5.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell5.transform);
                                break;

                            case "CodingCell6":
                                if(codingCell6.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell6.transform);
                                break;

                            case "CodingCell7":
                                if(codingCell7.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell7.transform);
                                break;

                            case "CodingCell8":
                                if(codingCell8.transform.childCount == 0)
                                    selectedCube.transform.SetParent(codingCell8.transform);
                                break;
                        }

                        selectedCube.GetComponent<BoxCollider>().enabled = false;
                        selectedCube.transform.localPosition = new Vector3(0f, 0f, 0f);
                        selectedCube.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                        selectedCube.transform.localScale = new Vector3(1f, 1f, 1f);
                        selectedCube = null;

                        source.clip = selectingCube;
                        source.Play();


                        inHands = false;
                    }
                }
                else
                {
                    if(selectedCube)
                    {
                        Debug.Log("DESTROYYYYYYYYY");
                        source.clip = releasingCube;
                        source.Play();
                        Destroy(selectedCube);
                        selectedCube = null;
                        inHands = false;
                        //Debug.Log("#### Raio hit e inHands: " + hit.transform.name + " --- " + inHands);

                    }
                }

            }
            else //instancia uma cópia do cubo selecionado colocando-o nas mãos do player ou apaga o cubo que está anexado na placa de programação
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.transform.name.Contains("CodingCell"))
                    {
                        source.clip = releasingCube;
                        source.Play();
                        switch(hit.transform.name)
                        {
                            case "CodingCell1":
                                Destroy(codingCell1.transform.GetChild(0).gameObject);
                                break;
                            case "CodingCell2":
                                Destroy(codingCell2.transform.GetChild(0).gameObject);
                                break;
                            case "CodingCell3":
                                Destroy(codingCell3.transform.GetChild(0).gameObject);
                                break;
                            case "CodingCell4":
                                Destroy(codingCell4.transform.GetChild(0).gameObject);
                                break;
                            case "CodingCell5":
                                Destroy(codingCell5.transform.GetChild(0).gameObject);
                                break;
                            case "CodingCell6":
                                Destroy(codingCell6.transform.GetChild(0).gameObject);
                                break;
                            case "CodingCell7":
                                Destroy(codingCell7.transform.GetChild(0).gameObject);
                                break;
                            case "CodingCell8":
                                Destroy(codingCell8.transform.GetChild(0).gameObject);
                                break;
                        }
                    }
                    else if(hit.transform.name.Contains("Dispenser"))
                    {
                        source.clip = selectingCube;
                        source.Play();
                        switch(hit.transform.name)
                        {
                            case "ForwardDispenser":
                                selectedCube = Object.Instantiate(forwardDispenser);
                                break;
                            case "BackDispenser":
                                selectedCube = Object.Instantiate(backDispenser);
                                break;
                            case "LeftDispenser":
                                selectedCube = Object.Instantiate(leftDispenser);
                                break;
                            case "RightDispenser":
                                selectedCube = Object.Instantiate(rightDispenser);
                                break;
                            case "BeginDispenser":
                                selectedCube = Object.Instantiate(beginDispenser);
                                break;
                            case "EndDispenser":
                                selectedCube = Object.Instantiate(endDispenser);
                                break;
                        }

                        Debug.Log("## O cubo selecionado foi o: " + selectedCube);

                        if(selectedCube)
                        {
                            selectedCube.transform.SetParent(playerHands.transform);
                            selectedCube.transform.localPosition = new Vector3(0f, -0.35f, 1.45f);
                            selectedCube.transform.rotation = Quaternion.Euler(72f, 0f, 0f);
                            selectedCube.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            inHands = true;
                        }

                        Debug.Log("#### Raio hit e inHands: " + hit.transform.name + " --- " + inHands);
                    }

                } //if ray

            }//if inhands

        } //if fire 

    }//update

}//class
