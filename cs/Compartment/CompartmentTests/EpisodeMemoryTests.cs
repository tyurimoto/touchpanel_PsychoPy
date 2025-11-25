using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compartment.Tests
{
    [TestClass()]
    public class EpisodeMemoryTests
    {
        [TestMethod()]
        public void EpisodeMemoryTest()
        {
            EpisodeMemory episodeMemory = new EpisodeMemory();
            Assert.IsNotNull(episodeMemory);
        }

        [TestMethod()]
        public void StoreShapObjectTest()
        {
            EpisodeMemory episodeMemory = new EpisodeMemory();
            ShapeObject[] shapeObjects = new ShapeObject[4];
            shapeObjects[0] = new ShapeObject();
            shapeObjects[1] = new ShapeObject();
            shapeObjects[2] = new ShapeObject();
            shapeObjects[3] = new ShapeObject();

            episodeMemory.AddOrUpdateShapObject("AA", shapeObjects);

            ShapeObject[] shapeObjects2 = new ShapeObject[4];
            shapeObjects2 = episodeMemory.ReadShapeObject("AA");
            Assert.AreEqual(shapeObjects2[0], shapeObjects[0]);
            Assert.AreEqual(shapeObjects2[1], shapeObjects[1]);
            Assert.AreEqual(shapeObjects2[2], shapeObjects[2]);
            Assert.AreEqual(shapeObjects2[3], shapeObjects[3]);

        }

        [TestMethod()]
        public void ReadShapeObjectTest()
        {
            EpisodeMemory episodeMemory = new EpisodeMemory();
            ShapeObject[] shapeObjects = new ShapeObject[4];
            shapeObjects[0] = new ShapeObject();
            shapeObjects[1] = new ShapeObject();
            shapeObjects[2] = new ShapeObject();
            shapeObjects[3] = new ShapeObject();

            episodeMemory.AddOrUpdateShapObject("AA", shapeObjects);

            ShapeObject[] shapeObjects2 = new ShapeObject[4];
            shapeObjects2 = episodeMemory.ReadShapeObject("AA");
            Assert.AreEqual(shapeObjects2[0], shapeObjects[0]);
            Assert.AreEqual(shapeObjects2[1], shapeObjects[1]);
            Assert.AreEqual(shapeObjects2[2], shapeObjects[2]);
            Assert.AreEqual(shapeObjects2[3], shapeObjects[3]);

        }

        [TestMethod()]
        public void AddEntryTest()
        {
            EpisodeMemory episodeMemory = new EpisodeMemory();
            ShapeObject[] shapeObjects = new ShapeObject[4];
            shapeObjects[0] = new ShapeObject();
            shapeObjects[1] = new ShapeObject();
            shapeObjects[2] = new ShapeObject();
            shapeObjects[3] = new ShapeObject();
            episodeMemory.AddEntry("AA", shapeObjects);

            //episodeMemory.StoreShapObject("AA", shapeObjects);
            ShapeObject[] shapeObjects2 = new ShapeObject[4];
            shapeObjects2 = episodeMemory.ReadShapeObject("AA");
            Assert.AreEqual(shapeObjects2[0], shapeObjects[0]);
            Assert.AreEqual(shapeObjects2[1], shapeObjects[1]);
            Assert.AreEqual(shapeObjects2[2], shapeObjects[2]);
            Assert.AreEqual(shapeObjects2[3], shapeObjects[3]);


        }

        [TestMethod()]
        public void RemoveEntryTest()
        {
            EpisodeMemory episodeMemory = new EpisodeMemory();
            ShapeObject[] shapeObjects = new ShapeObject[4];
            shapeObjects[0] = new ShapeObject();
            shapeObjects[1] = new ShapeObject();
            shapeObjects[2] = new ShapeObject();
            shapeObjects[3] = new ShapeObject();

            episodeMemory.AddOrUpdateShapObject("AA", shapeObjects);
            Assert.AreEqual(1, episodeMemory.GetCount());
            episodeMemory.RemoveEntry("AA");
            Assert.AreEqual(0, episodeMemory.GetCount());

        }

        [TestMethod()]
        public void ReadIncorrectShapeObjectsTest()
        {
            EpisodeMemory episodeMemory = new EpisodeMemory();
            ShapeObject[] shapeObjects = new ShapeObject[4];
            shapeObjects[0] = new ShapeObject();
            shapeObjects[1] = new ShapeObject();
            shapeObjects[2] = new ShapeObject();
            shapeObjects[3] = new ShapeObject();

            ShapeObject[] incorrectShapeObjects = new ShapeObject[3];

            episodeMemory.AddOrUpdateShapObject("AA", shapeObjects);
            Assert.AreEqual(1, episodeMemory.GetCount());
            Assert.AreEqual(shapeObjects[0], episodeMemory.ReadCorrectShapeObject("AA"));

            incorrectShapeObjects = episodeMemory.ReadIncorrectShapeObjects("AA");
            Assert.AreEqual(incorrectShapeObjects[0], shapeObjects[1]);
            Assert.AreEqual(incorrectShapeObjects[1], shapeObjects[2]);
            Assert.AreEqual(incorrectShapeObjects[2], shapeObjects[3]);
        }

        [TestMethod()]
        public void SaveKeysTest()
        {
            EpisodeMemory episodeMemory = new EpisodeMemory();
            ShapeObject[] shapeObjects = new ShapeObject[4];
            shapeObjects[0] = new ShapeObject();
            shapeObjects[1] = new ShapeObject();
            shapeObjects[2] = new ShapeObject();
            shapeObjects[3] = new ShapeObject();
            episodeMemory.AddEntry("AA", shapeObjects);

            episodeMemory.SaveKeys("a.json");

            EpisodeMemory episodeMemoryRead = new EpisodeMemory("a.json");

            ShapeObject[] readShapeObjects = episodeMemoryRead.ReadShapeObject("AA");

            for (int i = 0; i < readShapeObjects.Length; i++)
            {
                Assert.AreEqual(readShapeObjects[i].Point, shapeObjects[i].Point);
                Assert.AreEqual(readShapeObjects[i].Shape, shapeObjects[i].Shape);
                Assert.AreEqual(readShapeObjects[i].ShapeColor, shapeObjects[i].ShapeColor);
            }
        }
    }
}