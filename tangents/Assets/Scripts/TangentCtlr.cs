﻿using System.Collections;
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
  [SerializeField]
  private LineRenderer  lineC;
  [SerializeField]
  private Transform     ptA1;
  [SerializeField]
  private Transform     ptA2;
  [SerializeField]
  private Transform     ptB1;
  [SerializeField]
  private Transform     ptB2;
  [SerializeField]
  private Toggle        togDX0;
  [SerializeField]
  private Toggle        togDXR;
  [SerializeField]
  private Toggle        togDY0;
  [SerializeField]
  private GameObject    goFoundTangents;
  [SerializeField]
  private Text          txtBSqM4A;
  #pragma warning restore 0649
  
  void Update() {
  
    // calculate radius of the circle. assumes sqaure RT
    float radius = theCircle.rect.width / 2.0f;
    
    Vector2 tanPosA = Vector2.zero;
    Vector2 tanPosB = Vector2.zero;
    
    Vector3 tmp = thePoint.localPosition;
    tmp.x = (togDX0.isOn ? theCircle : thePoint).localPosition.x;
    tmp.x = (togDXR.isOn ? theCircle.localPosition.x + radius : tmp.x);
    tmp.y = (togDY0.isOn ? theCircle : thePoint).localPosition.y;
    thePoint.localPosition = tmp;
    
    bool foundTangents = CircleTangents_2((Vector2)theCircle.localPosition, radius, (Vector2)thePoint.localPosition, ref tanPosA, ref tanPosB);
//  bool foundTangents = ParabolaTangents((Vector2)theCircle.localPosition, 0.01f, (Vector2)thePoint.localPosition, ref tanPosA, ref tanPosB);

    goFoundTangents.SetActive(foundTangents);
    
    Vector2 tanExtrapolateA = tanPosA + (tanPosA - (Vector2)thePoint.localPosition);
    Vector2 tanExtrapolateB = tanPosB + (tanPosB - (Vector2)thePoint.localPosition);
    
    lineA.SetPosition(0, thePoint .localPosition);
    lineA.SetPosition(1, tanExtrapolateA);
    lineB.SetPosition(0, thePoint .localPosition);
    lineB.SetPosition(1, tanExtrapolateB);
    lineC.SetPosition(0, thePoint .localPosition);
    lineC.SetPosition(1, theCircle.localPosition);
    
    ptA1.localPosition = tanPosA;
    ptB1.localPosition = tanPosB;
    
    lineA.gameObject.SetActive(foundTangents);
    lineB.gameObject.SetActive(foundTangents);
    ptA1 .gameObject.SetActive(foundTangents);
    ptB1 .gameObject.SetActive(foundTangents);
  }
  
  // this approach is more geometrical and less algebraic than approach 1,
  // and far more stable. thanks to Mike Plotz for suggesting this direction.
  bool CircleTangents_2(Vector2 center, float r, Vector2 p, ref Vector2 tanPosA, ref Vector2 tanPosB) {
    p -= center;
    
    float P = p.magnitude;
    
    // if p is inside the circle, there ain't no tangents.
    if (P <= r) {
      return false;
    }
        
    float a = r * r                                          / P;    
    float q = r * (float)System.Math.Sqrt((P * P) - (r * r)) / P;
    
    Vector2 pN  = p / P;
    Vector2 pNP = new Vector2(-pN.y, pN.x);
    Vector2 va  = pN * a;
    
    tanPosA = va + pNP * q;
    tanPosB = va - pNP * q;

    tanPosA += center;
    tanPosB += center;
    
    return true;
  }
  
  
  bool CircleTangents_1(Vector2 center, float r, Vector2 p, ref Vector2 tanPosA, ref Vector2 tanPosB) {
    // subtract off the circle center
    p -= center;
    
    // according to my research,
    // the x-coordinates where tangencies occur are given by a quadratic with:   
    // a = Px^2 + Py^2
    // b = -2 * Px * R^2
    // c = R^2 * (R^2 - Py)
    // these values can be derived by any of three approaches:
    // 1. use the equation of a circle (x^2 + y^2 = r^2)
    //    together with the constraint that a tangent must be perpendicular to the radius,
    //    as expressed by the dot-product of the tangent and the radius being 0.
    // 2. same as 1, but express perpendicularity with the slopes being inverse reciprocals.
    // 3. calculate the slope at the point of tangency based on the derivative of the equation for y(x).
    
    double rr = r * r;
    double pypy = p.y * p.y;
    
    double a = (p.x * p.x) + (pypy);
    double b = -2.0f * p.x * rr;
    double c = rr * (rr - pypy);
    
    double BSquaredMinus4AC = (b * b) - (4f * a * c);
    
    txtBSqM4A.text = BSquaredMinus4AC.ToString("0.00");

    if (BSquaredMinus4AC <= 0f) {
      // all solutions are imaginary
      return false;
    }

    double x1 = (-b + System.Math.Sqrt(BSquaredMinus4AC)) / (2f * a);
    double x2 = (-b - System.Math.Sqrt(BSquaredMinus4AC)) / (2f * a);

    tanPosA.x = (float)x1;
    tanPosB.x = (float)x2;
    
    // decide whether to use the positive or negative solution.
    // just going by the pictures here.
    double mulA = 1f;
    mulA *= p.y > 0 ? 1f : -1f;
    double mulB = mulA;
    mulA *= p.x <  r ? 1f : -1f;
    mulB *= p.x > -r ? 1f : -1f;
    
    tanPosA.y = (float)(mulA * System.Math.Sqrt(rr - (x1 * x1)));    
    tanPosB.y = (float)(mulB * System.Math.Sqrt(rr - (x2 * x2)));
    
    // add on the circle center
    tanPosA += center;
    tanPosB += center;
    return true;
  }
  
  bool ParabolaTangents(Vector2 center, float k, Vector2 p, ref Vector2 tanPosA, ref Vector2 tanPosB) {
    // subtract off the circle center
    p -= center;
    
    // according to my research,
    // the x-coordinates where tangencies occur are given by a quadratic with:   
    // a = k
    // b = -2 * k * Px
    // c = Py
    // these values can be derived by any of three approaches:
    // 3. calculate the slope at the point of tangency based on the derivative of the equation for y(x).
    
    float a = k;
    float b = -2.0f * k * p.x;
    float c = p.y;
    
    float BSquaredMinus4AC = (b * b) - (4f * a * c);
    
    if (BSquaredMinus4AC < 0f) {
      // all solutions are imaginary
      return false;
    }

    float x1 = (-b + Mathf.Sqrt(BSquaredMinus4AC)) / (2f * a);
    float x2 = (-b - Mathf.Sqrt(BSquaredMinus4AC)) / (2f * a);

    tanPosA.x = x1;
    tanPosB.x = x2;
    
    tanPosA.y = k * x1 * x1;
    tanPosB.y = k * x2 * x2;
    
    // add on the circle center
    tanPosA += center;
    tanPosB += center;
    return true;
  }
}
