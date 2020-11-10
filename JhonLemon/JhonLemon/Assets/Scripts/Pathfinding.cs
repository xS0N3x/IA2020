using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public Grid GridReference;
    public Transform StartPosition;
    public Transform TargetPosition;
    public Transform PaulPosition;
    public SmartGhost SmartScript;

    private PaulMovement PaulActivation;
    private WaypointNavigator comeBack;
    private float Increment = 0f;

    private void Awake()
    {
        PaulActivation = GameObject.FindObjectOfType<PaulMovement>();
        SmartScript = gameObject.transform.Find("PointOfView").GetComponent<SmartGhost>();
    }

    private void Update()
    {
        Pathfind();
    }

    void FindPath(Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = GridReference.NodeFromWorldPoint(a_StartPos);
        Node TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);

        List<Node> OpenList = new List<Node>();
        HashSet<Node> ClosedList = new HashSet<Node>();

        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            Node CurrentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].ihCost < CurrentNode.ihCost)
                {
                    CurrentNode = OpenList[i];
                }
            }
            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode == TargetNode)
            {
                GetFinalPath(StartNode, TargetNode);
            }

            foreach (Node NeighborNode in GridReference.GetNeighboringNodes(CurrentNode))
            {
                if (!NeighborNode.bIsWall || ClosedList.Contains(NeighborNode))
                {
                    continue;
                }
                int MoveCost = CurrentNode.igCost + GetManhattenDistance(CurrentNode, NeighborNode);

                if (MoveCost < NeighborNode.igCost || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.igCost = MoveCost;
                    NeighborNode.ihCost = GetManhattenDistance(NeighborNode, TargetNode);
                    NeighborNode.ParentNode = CurrentNode;

                    if (!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
            }
        }
    }

    void GetFinalPath(Node a_StartingNode, Node a_EndNode) 
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = a_EndNode;

        while (CurrentNode != a_StartingNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.ParentNode;
        }

        FinalPath.Reverse();

        GridReference.FinalPath = FinalPath;

        foreach (Node node in GridReference.FinalPath)
        {
            transform.position = Vector3.MoveTowards(transform.position, FinalPath[0].vPosition, Time.deltaTime * 0.045f + Increment);
            Vector3 direction = FinalPath[0].vPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.5f * Time.deltaTime);
        }
        Increment += 0.00003f;
    }

    int GetManhattenDistance(Node a_nodeA, Node a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.iGridX - a_nodeB.iGridX);
        int iy = Mathf.Abs(a_nodeA.iGridY - a_nodeB.iGridY);

        return ix + iy;
    }

    void Pathfind() {
        if (PaulActivation.activePaul == false)//If Paul isn't active
        {
            if (SmartScript.m_IsPlayerInRange)// If Player is in range
            {
                FindPath(StartPosition.position, TargetPosition.position); // Go to player
            }
            else //Return to patrol behaviour
            {
                SmartScript.Patrol.enabled = true;
                SmartScript.Pathfinder.enabled = false;
            }

            //Look towards direction
            Vector3 direction = TargetPosition.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10f * Time.deltaTime);
        }
        else //If Paul is active
        {
            FindPath(StartPosition.position, PaulPosition.position); //Go to Paul
        }
    }
}
