import { useSession } from "./session_provider";
import { useEffect, useState } from "react";
import { Empty } from "../protobuf-javascript/google/protobuf/empty_pb.js";
import { CardTitle, CardDescription, CardHeader, CardContent, Card } from "@/components/ui/card";

export default function App() {
    const { grpcClients, isGrpcClientsSet } = useSession();
    const [message, setMessage] = useState("No message yet.");

    useEffect(() => {
        console.log("GrpcClients: ", grpcClients);

        const client = grpcClients["navClient"];

        if (!client) {
            return;
        }

        async function setMessageAsync() {
            const response = await new Promise((resolve, reject) => {
                client.sayHelloWorld(new Empty(), {}, (err, response) => {
                    if (err) reject(err);
                    else resolve(response);
                });
            });

            console.log("Response: ", response.toObject());

            response.toObject().message && setMessage(response.toObject().message);
        }

        setMessageAsync();
    }, [isGrpcClientsSet]);

    return (
        <div className="flex h-screen items-center justify-center bg-muted">
            <Card className="mx-auto mt-10 w-full max-w-md rounded-lg shadow-md" style={{ borderColor: "hsl(var(--border))" }}>
                <CardHeader className="text-center">
                    <CardTitle className="text-2xl font-bold">What does the server say?</CardTitle>
                </CardHeader>
                <CardContent>
                    <CardDescription className="text-center text-primary">{message}</CardDescription>
                </CardContent>
            </Card>
        </div>
    );
}
