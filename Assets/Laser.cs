using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : Pattern
{
    [SerializeField]
    private Transform m_hitPoint = null;
    [SerializeField]
    [Tooltip("피격 판정이 시작되기까지의 예열 시간")]
    private float m_preheatTime = 2f;

    private LineRenderer m_lineRenderer;
    private bool m_isHeating = false;
    private bool m_isAttacking = false;

    void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.enabled = false;
        m_lineRenderer.useWorldSpace = true;
        m_lineRenderer.startWidth = 0.1f;
        m_lineRenderer.endWidth = 0.1f;
        Color color = m_lineRenderer.startColor;
        color.a = 0f;
        m_lineRenderer.startColor = color;
        color = m_lineRenderer.endColor;
        color.a = 0f;
        m_lineRenderer.endColor = color;

        m_isHeating = false;
        m_isAttacking = false;
    }

    void Update()
    {
        if (m_isHeating)
        {
            int layerMask = 1 << 2;  // "Ignore Raycast" layer
            layerMask = ~layerMask;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, 20f, layerMask);
            m_lineRenderer.SetPosition(0, transform.position);
            m_lineRenderer.SetPosition(1, hit.point);
            m_hitPoint.position = hit.point;
            Debug.DrawLine(transform.position, hit.point);
            if (m_isAttacking && hit)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player HIT by laser");
                }
            }
        }
    }

    public override void StopPattern()
    {
        base.StopPattern();

        m_isHeating = false;
        m_isAttacking = false;
        m_lineRenderer.enabled = false;
    }

    protected override IEnumerator Fire()
    {
        Debug.Log("Laser Fire() called");
        yield return new WaitForSeconds(m_startDelay);

        m_isHeating = true;

        m_lineRenderer.enabled = true;
        float heatStartTime = Time.time;
        float heatEndTime = Time.time + m_preheatTime;
        while (Time.time < heatEndTime)
        {
            Color color = m_lineRenderer.startColor;
            float a = Mathf.Lerp(0f, 1f, (Time.time - heatStartTime) / m_preheatTime);
            color.a = a;
            m_lineRenderer.startColor = color;
            color = m_lineRenderer.endColor;
            color.a = a;
            m_lineRenderer.endColor = color;

            yield return new WaitForSeconds(0.01f);
        }

        m_isAttacking = true;
    }
}
