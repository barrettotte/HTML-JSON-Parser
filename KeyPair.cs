public class KeyPair{
    private string key;
    private string value;

    public string Key{ get{ return this.key; }}
    public string Value{ get{ return this.value; }}

    public KeyPair(){
        this.key = "";
        this.value = "";
    }
    public KeyPair(string k, string v){
        this.key = k;
        this.value = v;
    }

    public override string ToString(){
        return "{ key=" + this.key + ", value=" + this.value + " } ";
    }
}