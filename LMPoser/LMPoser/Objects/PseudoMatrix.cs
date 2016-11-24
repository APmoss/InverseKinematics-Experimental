using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace LMPoser.Objects {
	public class PseudoMatrix {
		Matrix<float> _mat;

		public PseudoMatrix(int rows, int columns) {
			_mat = CreateMatrix.Dense<float>(rows, columns);
		}

		public void SetRow(int index, float[] vector) {
			_mat.SetRow(index, vector);
		}
		public void SetColumn(int index, float[] vector) {
			_mat.SetColumn(index, vector);
		}
	}
}
