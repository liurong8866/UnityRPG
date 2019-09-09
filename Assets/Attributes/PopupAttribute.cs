using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MyLib {
//getPopList from target Object  GetOutPutTri
public class PopupAttribute : PropertyAttribute {
	public List<string> popList;
	public string name;
	public PopupAttribute(string n)
	{
		name = n;
	}

}
}

