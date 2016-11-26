using MathNet.Numerics.LinearAlgebra;

namespace LMPoser {
	public static class ExtensionMethods {
		public static Matrix<float> PseudoInverse(this Matrix<float> mat) {
			if (mat.RowCount > mat.ColumnCount) {
				return (mat.Transpose() * mat).Inverse() * mat.Transpose();
			}
			else if (mat.RowCount < mat.ColumnCount) {
				return mat.Transpose() * (mat * mat.Transpose()).Inverse();
			}
			else {
				return mat.Inverse();
			}
		}
	}
}
