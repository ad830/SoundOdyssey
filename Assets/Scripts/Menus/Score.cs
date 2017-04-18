using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable()]
public class Score : ISerializable {

	public int playerId;
	public byte levelId;
	public float expression;
	public float accuracy;
	public float fluency;
	public int totalNotesHit;
	public int totalNotes;
	public int totalScore;
/*	public float reserveFloat1;
	public float reserveFloat2;
	public int reserveInt1;
	public int reserveInt2;*/
	
	public Score(){
	
		playerId = 0;
		levelId = 0;
		expression = 0f;
		accuracy = 0f;
		fluency = 0f;
		totalNotesHit = 0;
		totalNotes = 0;
		totalScore = 0;
		
		/*reserveFloat1 = 999.99;
		reserveFloat2 = 999.99;
		reserveInt1 = 999;
		reserveInt2 = 999;*/
		
	}
	
	//Deserialization constructor.
	public Score(SerializationInfo info, StreamingContext ctxt)
	{
		//Get the values from info and assign them to the appropriate properties
		playerId = (int)info.GetValue("playerId", typeof(int));
		levelId = (byte)info.GetValue("levelId", typeof(byte));
		expression = (float)info.GetValue("expression",typeof(float));
		accuracy = (float)info.GetValue("accuracy",typeof(float));
		fluency = (float)info.GetValue("fluency",typeof(float));
		totalNotesHit = (int)info.GetValue("totalNotesHit",typeof(int));
		totalNotes = (int)info.GetValue("totalNotes",typeof(int));
		totalScore = (int)info.GetValue("totalScore",typeof(int));	
	}
	
	//Serialization function.
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("playerId", playerId);
		info.AddValue("levelId", levelId);
		info.AddValue("expression", expression);
		info.AddValue("accuracy", accuracy);
		info.AddValue("fluency", fluency);
		info.AddValue("totalNotesHit", totalNotesHit);
		info.AddValue("totalNotes", totalNotes);
		info.AddValue("totalScore", totalScore);
	}
	
	
}
