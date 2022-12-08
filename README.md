# Mafia SDK: Core IO

This library is based on [MafiaToolkit](https://github.com/Greavesy1899/MafiaToolkit). The objective is to (de)serialize SDS archives
from Mafia series games and provide an API to open/save different resource types contained in SDS archives.

Most of the code had been refactored and cleaned. All resource types (de)serialization process had been tested and validated, however this library do not support adding
resource yet (at least it has not been tested).

TODO:
- [ ] Write unit tests
- [X] Validate SDS archive (de)serialization 
- [ ] Memory Optimization 
  - [ ] Array pool of different object types (``byte``, ``Span``, ``MemoryStream``, etc)
  - [ ] A faster read/write process on binary file
  - [ ] A better implementation of ``DataWriterMonitor`` and ``DataWriterLogger``
  - [ ] Reduce memory allocation when possible 
  - [ ] Use pipes instead of streams ?
- [ ] Externalize Oodle/Zlib libraries (currently copy/pasted from their original repositories, this should need a proper fork of those repositories and publish nuget packages
and get clean dependencies links for this project (git submodules ?))
- [ ] Same for Gibbed.IO library which had been refactored (unit tests will be provided as well)
- [ ] Write a better readme file (move to GitHub project ?) 
- [ ] Write a CICD pipeline
- [ ] Restructure namespaces after libraries externalization 



 



