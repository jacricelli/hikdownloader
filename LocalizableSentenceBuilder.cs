namespace HikDownloader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Generador de oraciones localizable.
    /// </summary>
    public class LocalizableSentenceBuilder : SentenceBuilder
    {
        /// <summary>
        /// Obtiene un delegado que devuelve la palabra "obligatorio".
        /// </summary>
        public override Func<string> RequiredWord
        {
            get
            {
                return () => "Requerido.";
            }
        }

        /// <summary>
        /// Obtiene un delegado que devuelve la palabra 'grupo'.
        /// </summary>
        public override Func<string> OptionGroupWord
        {
            get
            {
                return () => "Grupo";
            }
        }

        /// <summary>
        /// Obtiene un delegado que devuelve el texto del encabezado del bloque de errores.
        /// </summary>
        public override Func<string> ErrorsHeadingText
        {
            // No puede ser pluralizado
            get
            {
                return () => "ERROR(ES):";
            }
        }

        /// <summary>
        /// Obtiene un delegado que devuelve el texto del encabezado del bloque de texto de uso.
        /// </summary>
        public override Func<string> UsageHeadingText
        {
            get
            {
                return () => "\nUSO:";
            }
        }

        /// <summary>
        /// Obtiene un delegado que devuelva el texto de ayuda del comando de ayuda.
        /// Los delegados deben aceptar un booleano que sea igual a <value>true</value> para las opciones; de lo contrario, <value>false</value> para verbos.
        /// </summary>
        public override Func<bool, string> HelpCommandText
        {
            get
            {
                return isOption => isOption
                    ? "Muestra esta pantalla de ayuda."
                    : "Muestra más información sobre un comando específico.";
            }
        }

        /// <summary>
        /// Obtiene un delegado que devuelva el texto de ayuda del comando vesion.
        /// Los delegados deben aceptar un booleano que sea igual a <value>true</value> para las opciones; de lo contrario, <value>false</value> para verbos.
        /// </summary>
        public override Func<bool, string> VersionCommandText
        {
            get
            {
                return _ => "Muestra información sobre la versión.";
            }
        }

        /// <summary>
        /// Obtiene una delegada que maneja el formato de error singular.
        /// Los delegados deben aceptar un <see cref="Error" /> y devolver una cadena.
        /// </summary>
        public override Func<Error, string> FormatError
        {
            get
            {
                return error =>
                {
                    switch (error.Tag)
                    {
                        case ErrorType.BadFormatTokenError:
                            return String.Format("No se reconoce el token '{0}'.", ((BadFormatTokenError)error).Token);

                        case ErrorType.MissingValueOptionError:
                            return String.Format("La opción '{0}' no tiene valor.", ((MissingValueOptionError)error).NameInfo.NameText);

                        case ErrorType.UnknownOptionError:
                            return String.Format("La opción '{0}' es desconocida.", ((UnknownOptionError)error).Token);

                        case ErrorType.MissingRequiredOptionError:
                            var errMisssing = ((MissingRequiredOptionError)error);
                            return errMisssing.NameInfo.Equals(NameInfo.EmptyName)
                                       ? "Falta un valor obligatorio que no está vinculado al nombre de la opción."
                                       : String.Format("Falta la opción obligatoria '{0}'.", errMisssing.NameInfo.NameText);

                        case ErrorType.BadFormatConversionError:
                            var badFormat = ((BadFormatConversionError)error);
                            return badFormat.NameInfo.Equals(NameInfo.EmptyName)
                                       ? "Un valor que no está vinculado al nombre de la opción se define con un formato incorrecto."
                                       : String.Format("La opción '{0}' está definida con un formato incorrecto.", badFormat.NameInfo.NameText);

                        case ErrorType.SequenceOutOfRangeError:
                            var seqOutRange = ((SequenceOutOfRangeError)error);
                            return seqOutRange.NameInfo.Equals(NameInfo.EmptyName)
                                       ? "Un valor de secuencia no vinculado al nombre de la opción se define con pocos elementos de los necesarios."
                                       : String.Format("Una opción de secuencia '{0}' se define con menos o más elementos de los necesarios.",
                                            seqOutRange.NameInfo.NameText);

                        case ErrorType.BadVerbSelectedError:
                            return String.Format("No se reconoce el verbo '{0}'.", ((BadVerbSelectedError)error).Token);

                        case ErrorType.NoVerbSelectedError:
                            return "Ningún verbo seleccionado.";

                        case ErrorType.RepeatedOptionError:
                            return String.Format("La opción '{0}' se define varias veces.", ((RepeatedOptionError)error).NameInfo.NameText);

                        case ErrorType.SetValueExceptionError:
                            var setValueError = (SetValueExceptionError)error;
                            return String.Format("Error al establecer el valor en la opción '{0}': {1}.",
                                setValueError.NameInfo.NameText,
                                setValueError.Exception.Message);

                        case ErrorType.MissingGroupOptionError:
                            var missingGroupOptionError = (MissingGroupOptionError)error;
                            return String.Format("Se requiere al menos una opción del grupo '{0}' ({1}).",
                                missingGroupOptionError.Group,
                                string.Join(", ", missingGroupOptionError.Names.Select(n => n.NameText)));

                        case ErrorType.GroupOptionAmbiguityError:
                            var groupOptionAmbiguityError = (GroupOptionAmbiguityError)error;
                            return String.Format("Tanto SetName como Group no están permitidos en la opción: {0}.",
                                groupOptionAmbiguityError.Option.NameText);

                        case ErrorType.MultipleDefaultVerbsError:
                            return "No se permite más de un verbo predeterminado.";
                    }

                    throw new InvalidOperationException();
                };
            }
        }

        /// <summary>
        /// Obtiene un delegado que controla el formato de errores de conjuntos mutuamente excluyentes.
        /// Los delegados deben aceptar una secuencia de <see cref="MutuallyExclusiveSetError" /> y devolver una cadena.
        /// </summary>
        public override Func<IEnumerable<MutuallyExclusiveSetError>, string> FormatMutuallyExclusiveSetErrors
        {
            get
            {
                return errors =>
                {
                    var bySet = from e in errors
                                group e by e.SetName into g
                                select new { SetName = g.Key, Errors = g.ToList() };

                    var msgs = bySet.Select(
                        set =>
                        {
                            var names = string.Join(
                                string.Empty,
                                (from e in set.Errors select "'".JoinTo(e.NameInfo.NameText, "', ")).ToArray());
                            var namesCount = set.Errors.Count();

                            var incompat = string.Join(
                                string.Empty,
                                (from x in
                                        (from s in bySet where !s.SetName.Equals(set.SetName) from e in s.Errors select e)
                                    .Distinct()
                                 select "'".JoinTo(x.NameInfo.NameText, "', ")).ToArray());

                            return
                                new StringBuilder("Opci")
                                        .AppendIf(namesCount > 1, "ones", "ón")
                                        .Append(": ")
                                        .Append(names.Substring(0, names.Length - 2))
                                        .Append(' ')
                                        .AppendIf(namesCount > 1, "no son", "no es")
                                        .Append(" compatible con: ")
                                        .Append(incompat.Substring(0, incompat.Length - 2))
                                        .Append('.')
                                    .ToString();
                        }).ToArray();

                    return string.Join(Environment.NewLine, msgs);
                };
            }
        }
    }
}