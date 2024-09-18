import { CardTitle, CardHeader, CardContent, Card } from "@/components/ui/card";
import Stiftungen from "./stiftungen.jsx";

export default function App() {
    return (
        <div className="flex h-screen items-center justify-center bg-muted">
            <Card className="mx-auto mt-10 w-full max-w-md rounded-lg shadow-md" style={{ borderColor: "hsl(var(--border))" }}>
                <CardHeader className="text-center">
                    <CardTitle className="text-2xl font-bold">What does the server say?</CardTitle>
                </CardHeader>
                <CardContent className="text-center text-lg text-primary">
                    <Stiftungen />
                </CardContent>
            </Card>
        </div>
    );
}
