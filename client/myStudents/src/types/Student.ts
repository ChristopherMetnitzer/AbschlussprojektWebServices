export default class Student {
    id: number;
    name: string;
    matrikelnummer: string;
    semester: number;

    constructor(id: number, name: string, matrikelnummer: string, semester: number) {
        this.id = id;
        this.name = name;
        this.matrikelnummer = matrikelnummer;
        this.semester = semester
    }
}