using NUnit.Framework;
using System;

namespace Asteroids.Test
{
    public class PlanarRotationTest
    {
        private const double DELTA = 1e-6;

        private const double ROT___0 = 0;
        private const double ROT__90 = Math.PI / 2;
        private const double ROT_180 = Math.PI;
        private const double ROT_270 = Math.PI + Math.PI / 2;

        private static readonly PlanarVector RIGHT = new PlanarVector(1, 0);
        private static readonly PlanarVector ___UP = new PlanarVector(0, 1);
        private static readonly PlanarVector _LEFT = new PlanarVector(-1, 0);
        private static readonly PlanarVector _DOWN = new PlanarVector(0, -1);


        [Test]
        public void StaticComponentRotationTest()
        {
            AssertUtil.AreEqual(RIGHT, PlanarRotation.Rotate(1, ROT___0), DELTA);
            AssertUtil.AreEqual(___UP, PlanarRotation.Rotate(1, ROT__90), DELTA);
            AssertUtil.AreEqual(_LEFT, PlanarRotation.Rotate(1, ROT_180), DELTA);
            AssertUtil.AreEqual(_DOWN, PlanarRotation.Rotate(1, ROT_270), DELTA);
        }

        [Test]
        public void StaticVectorRotationTest()
        {
            AssertUtil.AreEqual(RIGHT, PlanarRotation.Rotate(RIGHT, ROT___0), DELTA);
            AssertUtil.AreEqual(___UP, PlanarRotation.Rotate(RIGHT, ROT__90), DELTA);
            AssertUtil.AreEqual(_LEFT, PlanarRotation.Rotate(RIGHT, ROT_180), DELTA);
            AssertUtil.AreEqual(_DOWN, PlanarRotation.Rotate(RIGHT, ROT_270), DELTA);
        }

        [Test]
        public void ComponentRotationTest()
        {
            AssertUtil.AreEqual(RIGHT, new PlanarRotation(ROT___0).Rotate(1), DELTA);
            AssertUtil.AreEqual(___UP, new PlanarRotation(ROT__90).Rotate(1), DELTA);
            AssertUtil.AreEqual(_LEFT, new PlanarRotation(ROT_180).Rotate(1), DELTA);
            AssertUtil.AreEqual(_DOWN, new PlanarRotation(ROT_270).Rotate(1), DELTA);
        }

        [Test]
        public void ComponentInvertedRotationTest()
        {
            AssertUtil.AreEqual(RIGHT, new PlanarRotation(ROT___0).InvertedRotate(1), DELTA);
            AssertUtil.AreEqual(_DOWN, new PlanarRotation(ROT__90).InvertedRotate(1), DELTA);
            AssertUtil.AreEqual(_LEFT, new PlanarRotation(ROT_180).InvertedRotate(1), DELTA);
            AssertUtil.AreEqual(___UP, new PlanarRotation(ROT_270).InvertedRotate(1), DELTA);
        }

        [Test]
        public void VectorRotationTest()
        {
            AssertUtil.AreEqual(RIGHT, new PlanarRotation(ROT___0).Rotate(RIGHT), DELTA);
            AssertUtil.AreEqual(___UP, new PlanarRotation(ROT__90).Rotate(RIGHT), DELTA);
            AssertUtil.AreEqual(_LEFT, new PlanarRotation(ROT_180).Rotate(RIGHT), DELTA);
            AssertUtil.AreEqual(_DOWN, new PlanarRotation(ROT_270).Rotate(RIGHT), DELTA);
        }

        [Test]
        public void VectorInvertedRotationTest()
        {
            AssertUtil.AreEqual(RIGHT, new PlanarRotation(ROT___0).InvertedRotate(RIGHT), DELTA);
            AssertUtil.AreEqual(_DOWN, new PlanarRotation(ROT__90).InvertedRotate(RIGHT), DELTA);
            AssertUtil.AreEqual(_LEFT, new PlanarRotation(ROT_180).InvertedRotate(RIGHT), DELTA);
            AssertUtil.AreEqual(___UP, new PlanarRotation(ROT_270).InvertedRotate(RIGHT), DELTA);
        }
    }
}