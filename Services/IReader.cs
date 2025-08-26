using CsvHelper;

namespace AdminPortalV8.Services
{
    //
    // Summary:
    //     Defines methods used to read parsed data from a CSV file.
    public interface IReader : IReaderRow, System.IDisposable
    {
        //
        // Summary:
        //     Gets the parser.
        IParser Parser { get; }

        //
        // Summary:
        //     Enumerates the records hydrating the given record instance with row data. The
        //     record instance is re-used and not cleared on each enumeration. This only works
        //     for streaming rows. If any methods are called on the projection that force the
        //     evaluation of the IEnumerable, such as ToList(), the entire list will contain
        //     the same instance of the record, which is the last row.
        //
        // Parameters:
        //   record:
        //     The record to fill each enumeration.
        //
        // Type parameters:
        //   T:
        //     The type of the record.
        //
        // Returns:
        //     An System.Collections.Generic.IEnumerable`1 of records.
        IEnumerable<T> EnumerateRecords<T>(T record);
        //
        // Summary:
        //     Gets all the records in the CSV file and converts each to System.Type T. The
        //     Read method should not be used when using this.
        //
        // Type parameters:
        //   T:
        //     The System.Type of the record.
        //
        // Returns:
        //     An System.Collections.Generic.IList`1 of records.
        IEnumerable<T> GetRecords<T>();
        //
        // Summary:
        //     Gets all the records in the CSV file and converts each to System.Type T. The
        //     read method should not be used when using this.
        //
        // Parameters:
        //   anonymousTypeDefinition:
        //     The anonymous type definition to use for the records.
        //
        // Type parameters:
        //   T:
        //     The System.Type of the record.
        //
        // Returns:
        //     An System.Collections.Generic.IEnumerable`1 of records.
        IEnumerable<T> GetRecords<T>(T anonymousTypeDefinition);
        //
        // Summary:
        //     Gets all the records in the CSV file and converts each to System.Type T. The
        //     Read method should not be used when using this.
        //
        // Parameters:
        //   type:
        //     The System.Type of the record.
        //
        // Returns:
        //     An System.Collections.Generic.IEnumerable`1 of records.
        IEnumerable<object> GetRecords(Type type);
        //
        // Summary:
        //     Advances the reader to the next record. This will not read headers. You need
        //     to call CsvHelper.IReader.Read then CsvHelper.IReader.ReadHeader for the headers
        //     to be read.
        //
        // Returns:
        //     True if there are more records, otherwise false.
        bool Read();
        //
        // Summary:
        //     Advances the reader to the next record. This will not read headers. You need
        //     to call CsvHelper.IReader.ReadAsync then CsvHelper.IReader.ReadHeader for the
        //     headers to be read.
        //
        // Returns:
        //     True if there are more records, otherwise false.
        Task<bool> ReadAsync();
        //
        // Summary:
        //     Reads the header record without reading the first row.
        //
        // Returns:
        //     True if there are more records, otherwise false.
        bool ReadHeader();
    }
}
