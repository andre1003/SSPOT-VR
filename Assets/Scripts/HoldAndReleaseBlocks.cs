using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldAndReleaseBlocks : MonoBehaviour
{
    //o cubo que vamos pegar
    public GameObject cube;

    //maos do player que vao ser pai do cubo, que vai segurar o cubo
    public GameObject hands;

    //controlar se há algum cubo nas mãos do player
    bool inHands = false;

    //pegar a posiçao do cubo e depois que o soltar ele retornará para a posicao que estava
    Vector3 cubePosition;

    // Start is called before the first frame update
    void Start()
    {
        //posicao do cubo para devolver no mesmo lugar depois q soltar, clicar de novo
        cubePosition = cube.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            if (inHands) //solta o cubo para a posicao original
            {
                this.GetComponent<HoldAndReleaseBlocks>().enabled = false;
                cube.transform.SetParent(null);
                cube.transform.localPosition = cubePosition;
                inHands = false;
            }
            else //pega o cubo da posição dele para as mãos
            {                
                cube.transform.SetParent(hands.transform);
                cube.transform.localPosition = new Vector3(0f, -.55f, 1.2f);
                inHands = true;
            }
            
        }

        
    }
}
