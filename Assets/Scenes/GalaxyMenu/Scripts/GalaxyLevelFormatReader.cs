using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Midi;

namespace SoundOdyssey
{
	public static class GalaxyLevelFormatReader
	{
		/*
		[SerializeField]
		List<Sector> sectors;
		*/

        private static Campaign.Exam LoadExam(List<Campaign.Sector> sectors, XmlNode examNode)
        {
            int examId = int.Parse(examNode.Attributes["id"].Value);
            string examName = examNode.Attributes["name"].Value;

            XmlNode scalesSectionNode = examNode["ScalesSection"];
            XmlNode scalesListNode = scalesSectionNode["scales"];
            int numScales = scalesListNode.ChildNodes.Count;
            ScalesSection.Data scalesData = new ScalesSection.Data();
            scalesData.scales = new Scale[numScales];

            for (int a = 0; a < numScales; a++)
            {
                scalesData = LoadScale(scalesListNode, scalesData, a);
            }

            scalesData.octaveCount = int.Parse(scalesSectionNode["octaveCount"].InnerText);
            scalesData.motion = scalesSectionNode["motion"].InnerText;
            scalesData.direction = scalesSectionNode["direction"].InnerText;

            //Debug.LogFormat("Octaves {0} Motion {1} Direction {2}",
            //    scalesData.octaveCount,
            //    scalesData.motion,
            //    scalesData.direction
            //);

            List<Campaign.IExamSection> examSections = new List<Campaign.IExamSection>();
            ScalesSection scales = new ScalesSection(scalesData);
            examSections.Add(scales);

            return new Campaign.Exam(
                    new Campaign.Identifier(examId, examName),
                    examSections
            );
        }

        private static ScalesSection.Data LoadScale(XmlNode scalesListNode, ScalesSection.Data scalesData, int a)
        {
            XmlNode scaleNode = scalesListNode.ChildNodes[a];
            string rootNote = scaleNode["root"].InnerText;
            string pattern = scaleNode["pattern"].InnerText;

            ScalePattern scalePattern = null;
            for (int i = 0; i < Scale.Patterns.Length; i++)
            {
                if (pattern == Scale.Patterns[i].Name)
                {
                    scalePattern = Scale.Patterns[i];
                }
            }

            if (scalePattern == null)
            {
                Debug.Log("Invalid scale pattern!");
                Debug.Break();
            }
            
            scalesData.scales[a] = new Scale(new Note(rootNote), scalePattern);
            //Debug.LogFormat("Loaded Scale {0}", scalesData.scales[a].ToString());
            return scalesData;
        }

		public static List<Campaign.Sector> LoadSectorsFromResources(string path)
		{
			XmlDocument doc = new XmlDocument();
			TextAsset xmlAsset = Resources.Load<TextAsset>(path);
			doc.LoadXml(xmlAsset.text);

			List<Campaign.Sector> sectors = new List<Campaign.Sector>(doc.DocumentElement.ChildNodes.Count);

			for (int sector = 0; sector < doc.DocumentElement.ChildNodes.Count; sector++) {
				XmlNode sectorNode = doc.DocumentElement.ChildNodes[sector];
				int sectorId = int.Parse(sectorNode.Attributes["id"].Value);
				string sectorName = sectorNode.Attributes["name"].Value;
				int sectorStageCount = sectorNode.ChildNodes.Count;

				Campaign.Sector sectorData = new Campaign.Sector(new Campaign.Identifier(sectorId, sectorName), sectorStageCount);

				// Exam will always be last stage
				for (int stage = 0; stage < sectorStageCount - 1; stage++) {
					XmlNode stageNode = sectorNode.ChildNodes[stage];
					int stageId = int.Parse(stageNode.Attributes["id"].Value);
					string stageName = stageNode.Attributes["name"].Value;
					int stageLevelCount = stageNode.ChildNodes.Count;

					Campaign.Stage stageData = new Campaign.Stage(new Campaign.Identifier(stageId, stageName), stageLevelCount);
				
					for (int level = 0; level < stageLevelCount; level++) {
						XmlNode levelNode = stageNode.ChildNodes[level];
						int levelId = int.Parse(levelNode.Attributes["id"].Value);
						string levelName = levelNode.Attributes["name"].Value;
						//int levelChildCount = levelNode.ChildNodes.Count;

						string levelTypeStr = levelNode["levelType"].InnerText;

						Campaign.Level.LevelType levelType = Campaign.Level.LevelType.Teaching;
						if (levelTypeStr == "Teaching")
						{
							levelType = Campaign.Level.LevelType.Teaching;
						}
						if (levelTypeStr == "Song")
						{
							levelType = Campaign.Level.LevelType.Song;
						}
						Campaign.Level levelData = new Campaign.Level(new Campaign.Identifier(levelId, levelName), levelType);
						switch (levelType) {
							case Campaign.Level.LevelType.Teaching:
								string sceneFilename = levelNode.ChildNodes[1].InnerText;
								levelData.teaching = new Campaign.Level.Teaching(sceneFilename);
							break;
							case Campaign.Level.LevelType.Song:
								string midiFilename = levelNode.ChildNodes[1].InnerText;
								int idealTrack = int.Parse(levelNode.ChildNodes[2].InnerText);
								levelData.song = new Campaign.Level.Song(midiFilename, idealTrack);
							break;
						}
						
						stageData.levels.Add(levelData);
					}
					sectorData.stages.Add(stageData);
				}
				XmlNode examNode = sectorNode.ChildNodes[sectorStageCount - 1];
                Campaign.Exam examData = LoadExam(sectors, examNode);

				sectorData.exam = examData;

				sectors.Add(sectorData);
			}

			return sectors;
		}

        //// Use this for initialization
        //void Start () {
        //    LoadSectorsFromResources("Levels/LevelStructure");
        //}
	}

}
