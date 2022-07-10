using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Movement speed modifier.")]
    [SerializeField] private float speed = 3;

    private Node currentNode;
    private Vector3 currentDir;
    private bool playerCaught = false;


    public delegate void GameEndDelegate();
    public event GameEndDelegate GameOverEvent = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        InitializeAgent();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCaught == false)
        {
            if (currentNode != null)
            {
                
                //If beyond 0.25 units of the current node.
                if (Vector3.Distance(transform.position, currentNode.transform.position) > 0.25f)
                {
                    transform.Translate(currentDir * speed * Time.deltaTime);
                }
                //Implement path finding here

                else
                {
                    //find new target node
                    var targetNode = DepthFirstSearch();


                    if (targetNode != currentNode && targetNode != null)                     //if target node is not the AIs current node and target node is not null
                    {
                        currentNode = targetNode;                           //set current node to target node
                    }
                    if (GameManager.Instance.Player.TargetNode != null && GameManager.Instance.Player.TargetNode != currentNode)                     //else if player target node is not null and player target node not current node
                    {
                        currentNode = GameManager.Instance.Player.TargetNode;                            //set current node to players target node
                    }

                    if(currentNode != null)                     //if current node is not null
                    {
                        currentDir = currentNode.transform.position - transform.position;                         //set current direction towards node
                        currentDir = currentDir.normalized;                         //normalize current direction

                    }
                }   
            }
            else
            {
                Debug.LogWarning($"{name} - No current node");
            }

            Debug.DrawRay(transform.position, currentDir, Color.cyan);
        }
    }

    //Called when a collider enters this object's trigger collider.
    //Player or enemy must have rigidbody for this to function correctly.
    private void OnTriggerEnter(Collider other)
    {
        if (playerCaught == false)
        {
            if (other.tag == "Player")
            {
                playerCaught = true;
                GameOverEvent.Invoke(); //invoke the game over event
            }
        }
    }

    /// <summary>
    /// Sets the current node to the first in the Game Managers node list.
    /// Sets the current movement direction to the direction of the current node.
    /// </summary>
    void InitializeAgent()
    {
        currentNode = GameManager.Instance.Nodes[0];
        currentDir = currentNode.transform.position - transform.position;
        currentDir = currentDir.normalized;

    }

    //Implement DFS algorithm method here
    private Node DepthFirstSearch()
    {
        Stack nodeStack = new Stack(); //Stacks the unvisited nodes, last one stacked is first popped
        List<Node> visitedNodes = new List<Node>(); //tracks the visited nodes
        nodeStack.Push(GameManager.Instance.Nodes[0]);

        while (nodeStack.Count > 0) //while the stack is not empty
        {
            Node currentNode = (Node)nodeStack.Pop();  //get the last stacked node
            visitedNodes.Add(currentNode); //mark current node as visited
            foreach (Node child in currentNode.Children)//loop through each child of current node
            {
                if (child == GameManager.Instance.Player.CurrentNode) //check if this child is equal to players current node
                {
                    return child;
                    //if so return child
                }
                nodeStack.Push(child); //push child to node
            }
        }
        return null; //target not found

    }

}
