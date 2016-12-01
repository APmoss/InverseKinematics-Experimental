using MathNet.Numerics.LinearAlgebra;
using System;

namespace LMPoser {
	public static class ExtensionMethods {
		public static Matrix<float> GeneralizedInverse(this Matrix<float> mat) {
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

		public static Matrix<float> SVDPseudoInverse(this Matrix<float> mat) {
			if (mat.RowCount < mat.ColumnCount) {
				var svd = mat.Svd();

				var u = svd.U;
				var dVec = svd.S;
				var dStar = CreateMatrix.Diagonal(mat.RowCount, mat.RowCount, (x) => dVec[x] < 10 ? 0f : 1f / dVec[x]);

				var vt = svd.VT;

				while (vt.RowCount > mat.RowCount) {
					vt = vt.RemoveRow(mat.RowCount);
				}

				// V D* UT
				return vt.Transpose() * dStar * u.Transpose();
			}
			else {
				throw new NotImplementedException();
			}
		}
	}
}
