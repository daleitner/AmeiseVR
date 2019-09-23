using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class MyCalendar : MonoBehaviour
{
	private DateTime _today;
	private TextMeshPro LblMonth;
	private TextMeshPro LblDay;
	private TextMeshPro LblYear;
	public GameObject Month;
	public GameObject Day;
	public GameObject Year;
	// Start is called before the first frame update
	void Start()
    {
		LblMonth = Month.GetComponent<TextMeshPro>();
		LblDay = Day.GetComponent<TextMeshPro>();
		LblYear = Year.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
	    if (KnowledgeBase.Instance.Date != _today)
	    {
		    _today = KnowledgeBase.Instance.Date;
		    LblMonth.text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_today.Month);
		    LblDay.text = _today.Day.ToString();
		    LblYear.text = _today.Year.ToString();
	    }
    }
}
