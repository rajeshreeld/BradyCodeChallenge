**Overview**

This is a C# console application developed as part of the coding challenge. The application processes XML files containing generator data, performs the necessary calculations, and generates an output XML file in the specified format. The key calculations include:

Total Generation Value for each generator.
The generator with the highest Daily Emissions for each day.
Actual Heat Rate for each coal generator.

The application uses a static reference data file (ReferenceData.xml) to get values like ValueFactor and EmissionsFactor. The input and output folder locations are configurable through the app.config file.

**Key Features:**

Total Generation Value Calculation: Energy * Price * ValueFactor
Daily Emissions Calculation: Energy * EmissionRating * EmissionFactor
Actual Heat Rate Calculation: TotalHeatInput / ActualNetGeneration

**Files**:

Input Folder: Contains XML files with generator data (.xml files).
Output Folder: Contains generated result files in XML format with the calculated values (_result.xml).
ReferenceData.xml: Static data used for calculations (e.g., ValueFactor, EmissionsFactor).

**Installation**

Clone this repository to your local machine:

git clone https://github.com/rajeshreeld/BradyGeneratorProcessor.git
Open the solution in Visual Studio or your preferred C# IDE.

Make sure that the necessary input and output folders are correctly configured in app.config.

**Usage**

Run the console application, and it will process any XML files placed in the input folder.
Note- It will not process existing files, only newly added files.
The application will calculate and output the results in the output folder, appending _result to the filename.

**Configuration**

Input Folder: Path to the folder containing the XML input files. This is configured in the app.config file.
Output Folder: Path to the folder where result files will be saved. This is also configurable in the app.config file.

**To-Do**

Unit Tests: Writing unit test cases for key functions is a planned task to ensure code correctness and reliability. This will be implemented in the future.

**License**

This project is licensed under the MIT License.
