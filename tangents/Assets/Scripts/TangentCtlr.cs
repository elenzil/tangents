using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TangentCtlr : MonoBehaviour {
  #pragma warning disable 0649
  [SerializeField]
  private Transform     thePoint;
  [SerializeField]
  private RectTransform theCircle;
  [SerializeField]
  private LineRenderer  lineA;
  [SerializeField]
  private LineRenderer  lineB;
  #pragma warning restore 0649
  
  void Update() {
  
    // calculate radius of the circle. assumes sqaure RT
    float radius = theCircle.rect.width / 2.0f;
    
  
    Vector2 tanPosA = Vector2.zero;
    Vector2 tanPosB = Vector2.zero;
    
    bool foundTangents = tangents((Vector2)theCircle.localPosition, radius, (Vector2)thePoint.localPosition, ref tanPosA, ref tanPosB);
    
    lineA.SetPosition(0, thePoint .localPosition);
    lineA.SetPosition(1, tanPosA);
    lineB.SetPosition(0, thePoint .localPosition);
    lineB.SetPosition(1, tanPosB);
    
    lineA.gameObject.SetActive(foundTangents);
    lineB.gameObject.SetActive(foundTangents);
  }
  
  bool tangents(Vector2 c, float r, Vector2 p, ref Vector2 tanPosA, ref Vector2 tanPosB) {
    return false;
  }
}
