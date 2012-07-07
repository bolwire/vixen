﻿using System;
using System.Linq;
using System.IO;
using Vixen.Sys;

namespace Vixen.Module.SequenceType {
	class SequenceTypeModuleManagement : GenericModuleManagement<ISequenceTypeModuleInstance> {

		/// <summary>
		/// Creates a new instance.  Does not load any existing content.
		/// </summary>
		/// <param name="sequenceFileType"></param>
		/// <returns></returns>
		public ISequenceTypeModuleInstance Get(string sequenceFileType) {
			string fileType = Path.GetExtension(sequenceFileType);
			ISequenceTypeModuleInstance sequenceType = Modules.GetRepository<ISequenceTypeModuleInstance>().GetAll().Cast<ISequenceTypeModuleInstance>().FirstOrDefault(x => x.FileExtension.Equals(fileType, StringComparison.OrdinalIgnoreCase));
			return sequenceType;
		}

		public ISequenceTypeModuleInstance GetFactory(ISequence sequence) {
			return Modules.GetRepository<ISequenceTypeModuleInstance>().GetAll().Cast<ISequenceTypeModuleInstance>().FirstOrDefault(x => x.CreateSequence().GetType() == sequence.GetType());
		}
	}
}
