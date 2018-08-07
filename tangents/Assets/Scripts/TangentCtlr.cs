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
  #pragma warning restore 0649
  
  void Update() {
  
    // calculate radius of the circle. assumes sqaure RT
    float radius = theCircle.rect.width / 2.0f;
    
  
    Vector2 tanPosA = Vector2.zero;
    Vector2 tanPosB = Vector2.zero;
    
    bool foundTangents = CircleTangents((Vector2)theCircle.localPosition, radius, (Vector2)thePoint.localPosition, ref tanPosA, ref tanPosB);
//  bool foundTangents = ParabolaTangents((Vector2)theCircle.localPosition, 0.01f, (Vector2)thePoint.localPosition, ref tanPosA, ref tanPosB);
    
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
  
  bool CircleTangents(Vector2 center, float r, Vector2 p, ref Vector2 tanPosA, ref Vector2 tanPosB) {
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
    
    float rr = r * r;
    float pypy = p.y * p.y;
    
    float a = (p.x * p.x) + (pypy);
    float b = -2.0f * p.x * rr;
    float c = rr * (rr - pypy);
    
    float BSquaredMinus4AC = (b * b) - (4f * a * c);

    if (BSquaredMinus4AC < 0f) {
      // all solutions are imaginary
      return false;
    }

    float x1 = (-b + Mathf.Sqrt(BSquaredMinus4AC)) / (2f * a);
    float x2 = (-b - Mathf.Sqrt(BSquaredMinus4AC)) / (2f * a);

    tanPosA.x = x1;
    tanPosB.x = x2;
    
    // decide whether to use the positive or negative solution.
    // just going by the pictures here.
    float mulA = 1f;
    mulA *= p.y > 0 ? 1f : -1f;
    float mulB = mulA;
    mulA *= p.x <  r ? 1f : -1f;
    mulB *= p.x > -r ? 1f : -1f;
    
    tanPosA.y = mulA * Mathf.Sqrt(rr - (x1 * x1));    
    tanPosB.y = mulB * Mathf.Sqrt(rr - (x2 * x2));    
    
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
