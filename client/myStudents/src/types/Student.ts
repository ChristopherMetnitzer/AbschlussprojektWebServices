export default class Student {
    id: number;
    name: string;
    matrikelnummer: string;
    semester: number;
    university?: string;

    constructor(id: number, name: string, matrikelnummer: string, semester: number, university?: string) {
        this.id = id;
        this.name = name;
        this.matrikelnummer = matrikelnummer;
        this.semester = semester;
        this.university = university;
    }
}