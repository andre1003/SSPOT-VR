using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunCubesBackup : MonoBehaviour
{
    public GameObject errorScreen;



    //para movimentar o robo
    public GameObject robot;
    private Animator robotAnimator;
    private AudioSource robotSource;

    //placas de programacao que o cubo pode ser anexado
    public List<GameObject> codingCell = new List<GameObject>();

    public GameObject terminal;

    public GameObject projector;

    public GameObject elevatorButton;
    public GoingUpAndDownController scriptGoingUpAndDown;

    public GameObject instructionsNextScene;

    public Material successTerminalMaterial;
    public Material errorTerminalMaterial;

    Material[] mats;    

    private AudioSource source;
    public AudioClip success;
    public AudioClip error;

    // Start is called before the first frame update
    void Start()
    {
        robotAnimator = robot.GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        robotSource = robot.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    { 
    }

    public void Run()
    {

        //acessar cada codingCell e verificar se há algum filho, se tiver, armazenar o nome do gameobject filho na lista cubes no formato de instrução, e printar isso

        List<string> cubes = new List<string>();
        string cube;
        int length=0;

        //compilador
        for (int i = 0; i < codingCell.Count; i++)
        {
            Debug.Log("%%%%%%%%%%%%%%%%%%%" + codingCell[i]);
           
            //verificar se todos estao preenchidos sequencialmente, nao pode ter nenhum buraco
            if (codingCell[i].transform.childCount > 0)
            {
                
                length = codingCell[i].transform.GetChild(0).gameObject.name.Length;
                cube = codingCell[i].transform.GetChild(0).gameObject.name.Remove(length-16);
                cubes.Add(cube);

            } else
            {
                //tem celula sem child, sem cubo, placa nao preenchida
                //mudar a cor de fundo do terminal e toca som de erro 
                source.clip = error;
                source.Play();
                mats = terminal.GetComponent<MeshRenderer>().materials;
                mats[1] = errorTerminalMaterial;
                terminal.GetComponent<MeshRenderer>().materials = mats;
                
                errorScreen.transform.GetComponentInChildren<Text>().text= "Deu ERRO ! Você deve preencher todas as placas de programação !";
                errorScreen.SetActive(true);
                
                break;
            }


            //primeiro = inicio
            if (i == 0 && cubes[i] != "Begin")
            {
                source.clip = error;
                source.Play();
                mats = terminal.GetComponent<MeshRenderer>().materials;
                mats[1] = errorTerminalMaterial;
                terminal.GetComponent<MeshRenderer>().materials = mats;

                errorScreen.transform.GetComponentInChildren<Text>().text = "Deu ERRO ! Verifique se o algoritmo foi iniciado corretamente !";
                errorScreen.SetActive(true);

                break;
            }

            //ultimo = fim
            if (i == (codingCell.Count - 1) && cubes[i] != "End")
            {
                source.clip = error;
                source.Play();
                mats = terminal.GetComponent<MeshRenderer>().materials;
                mats[1] = errorTerminalMaterial;
                terminal.GetComponent<MeshRenderer>().materials = mats;
                
                errorScreen.transform.GetComponentInChildren<Text>().text = "Deu ERRO ! Verifique se o algoritmo foi finalizado corretamente !";
                errorScreen.SetActive(true);
                
                break;
            }

            if ( (i!= 0) && (i != codingCell.Count - 1) && (cubes[i] == "Begin" || cubes[i] == "End"))
            {
                source.clip = error;
                source.Play();
                mats = terminal.GetComponent<MeshRenderer>().materials;
                mats[1] = errorTerminalMaterial;
                terminal.GetComponent<MeshRenderer>().materials = mats;

                errorScreen.transform.GetComponentInChildren<Text>().text = "Deu ERRO ! Início e Fim devem ser usados no lugar certo !";
                errorScreen.SetActive(true); 

                break;
            }

            if (i == (codingCell.Count-1))
            {
                source.clip = success;
                source.Play();
                
                mats = terminal.GetComponent<MeshRenderer>().materials;
                mats[1] = successTerminalMaterial;
                terminal.GetComponent<MeshRenderer>().materials = mats;

                robotAnimator.SetBool("Start", true);
                robotSource.Play();
                instructionsNextScene.SetActive(true);
                projector.SetActive(true);
                errorScreen.SetActive(false);
                
                //scriptGoingUpAndDown.GoingUpAndDown();
                
            }

            Debug.Log("&&&&&& CUBO " + i + ": " + cubes[i]);

        } //for compilador

       

    }

    

}
