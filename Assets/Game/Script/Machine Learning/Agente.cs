using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// O Agente requer um Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class Agente : MonoBehaviour
{
    /// <summary>
    /// Velocidade de movimento do Agente
    /// </summary>
    public float velocity = 100f;

    // Identifica o componente Rigidbody do nosso Agente
    private Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    float yr = 0f;
    /// Executa a cada 0.02 segundos
    private void FixedUpdate() {
            // Mover o agente para cima.
            if (Input.GetKey(KeyCode.UpArrow)) rb.AddForce(transform.up * velocity * Time.fixedDeltaTime);
            else ReturnPositionY(1.5f);

            // Mover o agente para baixo
            if (Input.GetKey(KeyCode.DownArrow)) rb.AddForce(-transform.up * velocity * Time.fixedDeltaTime);
            else ReturnPositionY(1.5f);

            // Mover o agente para frente
            if (Input.GetKey(KeyCode.W)) rb.AddForce(transform.forward * velocity * Time.fixedDeltaTime);

            // Mover o agente para trás
            if (Input.GetKey(KeyCode.S)) rb.AddForce(-transform.forward * velocity * Time.fixedDeltaTime);

            // Rotaciona o agente para direita
            if (Input.GetKey(KeyCode.D)) transform.Rotate(0f, velocity * Time.fixedDeltaTime, 0f, Space.Self);
            
            // Rotaciona o agente para esquerda
            if (Input.GetKey(KeyCode.A)) transform.Rotate(0f, -velocity * Time.fixedDeltaTime, 0f, Space.Self);

            // Rotaciona o agente para cima
            if (Input.GetKey(KeyCode.Q)) transform.Rotate(-velocity * Time.fixedDeltaTime, 0f, 0f);
            
            // Rotaciona o agente para baixo
            else if (Input.GetKey(KeyCode.E)) transform.Rotate(velocity * Time.fixedDeltaTime, 0f, 0f);
            else ReturnRotationY(transform.rotation.y * 180);
    }

    // Retorna a posição Y indicada
    private void ReturnPositionY(float y){
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, y, transform.position.z), 0.01f);
    }

    // Retorna a rotação 0 nos eixos X e Z e mantém o valor de Y
    private void ReturnRotationY(float y){
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, y, 0f), 0.1f);
    }
}
